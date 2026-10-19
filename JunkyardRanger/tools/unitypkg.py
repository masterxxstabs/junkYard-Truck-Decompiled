"""Reads an AssetRipper-exported .unitypackage (already untarred): prefabs, meshes,
materials and textures, in Unity's text serialization."""
import os, re, struct, math
import yaml

DOC = re.compile(r"^--- !u!(\d+) &(-?\d+)[^\n]*\n", re.M)
BUILTIN_GUID = "0000000000000000e000000000000000"


class Package:
    def __init__(self, raw):
        self.raw = raw
        self.by_guid = {}
        self.by_path = {}
        for g in os.listdir(raw):
            pn = os.path.join(raw, g, "pathname")
            if not os.path.exists(pn):
                continue
            path = open(pn, encoding="utf-8", errors="replace").read().splitlines()[0].strip()
            asset = os.path.join(raw, g, "asset")
            self.by_guid[g] = (path, asset if os.path.exists(asset) else None)
            self.by_path[path] = g
        self._mesh_cache = {}
        self._mat_cache = {}

    def asset(self, guid):
        e = self.by_guid.get(guid)
        return e[1] if e else None

    def path(self, guid):
        e = self.by_guid.get(guid)
        return e[0] if e else None

    # ---------------------------------------------------------------- YAML docs
    def docs(self, guid):
        text = open(self.asset(guid), encoding="utf-8", errors="replace").read()
        out = {}
        ms = list(DOC.finditer(text))
        for i, m in enumerate(ms):
            body = text[m.end(): ms[i + 1].start() if i + 1 < len(ms) else len(text)]
            try:
                data = yaml.load(body, Loader=yaml.CSafeLoader if hasattr(yaml, "CSafeLoader") else yaml.SafeLoader)
            except Exception as e:
                data = {"_error": str(e)}
            if isinstance(data, dict) and len(data) == 1:
                (kind, fields), = data.items()
                out[int(m.group(2))] = (int(m.group(1)), kind, fields or {})
        return out

    # ---------------------------------------------------------------- meshes
    def mesh(self, ref):
        """ref: {fileID, guid}. Returns dict(name, positions, normals, uvs, submeshes[list of tri lists]) or None."""
        guid = ref.get("guid")
        fid = ref.get("fileID")
        key = (guid, fid)
        if key in self._mesh_cache:
            return self._mesh_cache[key]
        result = None
        if guid == BUILTIN_GUID:
            result = {"builtin": {10202: "Cube", 10206: "Cylinder", 10207: "Sphere", 10208: "Capsule", 10209: "Plane", 10210: "Quad"}.get(fid, "Cube")}
        elif guid and self.asset(guid):
            try:
                result = parse_mesh(open(self.asset(guid), encoding="utf-8", errors="replace").read())
            except Exception as e:
                print("mesh", self.path(guid), "unreadable:", e)
                result = None
        self._mesh_cache[key] = result
        return result

    # ---------------------------------------------------------------- materials
    def material(self, ref):
        guid = ref.get("guid")
        if guid in self._mat_cache:
            return self._mat_cache[guid]
        result = None
        if guid and self.asset(guid):
            d = self.docs(guid)
            for cls, kind, f in d.values():
                if kind == "Material":
                    result = read_material(f, self)
                    break
        self._mat_cache[guid] = result
        return result


def read_material(f, pkg):
    m = {"name": f.get("m_Name", "mat"), "shader": None, "tex": {}, "floats": {}, "colors": {}, "keywords": f.get("m_ShaderKeywords", "")}
    sh = f.get("m_Shader") or {}
    m["shader"] = (pkg.path(sh.get("guid")) if sh.get("guid") else None, sh.get("fileID"))
    props = f.get("m_SavedProperties") or {}

    def entries(x):
        if isinstance(x, dict):
            return list(x.items())
        out = []
        for item in x or []:
            if isinstance(item, dict):
                if "first" in item:
                    out.append((item["first"], item.get("second")))
                else:
                    out += list(item.items())
        return out

    for k, v in entries(props.get("m_TexEnvs")):
        if True:
            t = (v or {}).get("m_Texture") or {}
            if t.get("guid"):
                path = pkg.path(t["guid"])
                m["tex"][k] = {"guid": t["guid"], "path": path, "file": pkg.asset(t["guid"]), "scale": v.get("m_Scale"), "offset": v.get("m_Offset")}
    for k, v in entries(props.get("m_Floats")):
        m["floats"][k] = v
    for k, v in entries(props.get("m_Colors")):
        m["colors"][k] = v
    return m


SIZES = {0: 4, 1: 2, 2: 1, 3: 1, 4: 2, 5: 2, 6: 1, 7: 1, 8: 2, 9: 2, 10: 4, 11: 4}


def _decode(buf, at, fmt):
    if fmt == 0:
        return struct.unpack_from("<f", buf, at)[0]
    if fmt == 1:
        return struct.unpack_from("<e", buf, at)[0]
    if fmt == 2:
        return buf[at] / 255.0
    if fmt == 3:
        return max(struct.unpack_from("<b", buf, at)[0] / 127.0, -1.0)
    if fmt == 4:
        return struct.unpack_from("<H", buf, at)[0] / 65535.0
    if fmt == 5:
        return max(struct.unpack_from("<h", buf, at)[0] / 32767.0, -1.0)
    if fmt in (6,):
        return buf[at]
    if fmt in (7,):
        return struct.unpack_from("<b", buf, at)[0]
    if fmt == 8:
        return struct.unpack_from("<H", buf, at)[0]
    if fmt == 9:
        return struct.unpack_from("<h", buf, at)[0]
    if fmt == 10:
        return struct.unpack_from("<I", buf, at)[0]
    return struct.unpack_from("<i", buf, at)[0]


def parse_mesh(text):
    name = re.search(r"^  m_Name: (.*)$", text, re.M)
    vd = text[text.index("m_VertexData:"):]
    vcount = int(re.search(r"m_VertexCount: (\d+)", vd).group(1))
    chans = [tuple(int(x) for x in m) for m in re.findall(r"- stream: (\d+)\n\s+offset: (\d+)\n\s+format: (\d+)\n\s+dimension: (\d+)", vd)]
    data_m = re.search(r"_typelessdata: ([0-9a-f]*)", vd)
    data = bytes.fromhex(data_m.group(1)) if data_m else b""
    if vcount == 0 or not data:
        return {"name": name.group(1) if name else "mesh", "empty": True}
    # Streams follow each other, each 16-byte aligned.
    streams = {}
    for s, off, fmt, dim in chans:
        if dim == 0:
            continue
        end = off + SIZES[fmt] * (dim & 0xF)
        streams[s] = max(streams.get(s, 0), end)
    stream_start = {}
    pos = 0
    for s in sorted(streams):
        stream_start[s] = pos
        pos += streams[s] * vcount
        pos = (pos + 15) // 16 * 16

    def channel(i):
        if i >= len(chans):
            return None
        s, off, fmt, dim = chans[i]
        dim &= 0xF
        if dim == 0:
            return None
        stride = streams[s]
        base = stream_start[s] + off
        size = SIZES[fmt]
        out = []
        for v in range(vcount):
            at = base + v * stride
            out.append(tuple(_decode(data, at + k * size, fmt) for k in range(dim)))
        return out

    positions = channel(0)
    normals = channel(1)
    uvs = channel(4)
    ib = bytes.fromhex(re.search(r"m_IndexBuffer: ([0-9a-f]*)", text).group(1))
    fmt32 = re.search(r"m_IndexFormat: (\d)", text).group(1) == "1"
    isz = 4 if fmt32 else 2
    subs = []
    for m in re.finditer(r"firstByte: (\d+)\n\s+indexCount: (\d+)\n\s+topology: (\d+)\n\s+baseVertex: (\d+)", text):
        first, count, topo, basev = (int(x) for x in m.groups())
        if topo != 0:
            subs.append([])
            continue
        idx = struct.unpack_from("<%d%s" % (count, "I" if fmt32 else "H"), ib, first)
        subs.append([i + basev for i in idx])
    return {"name": name.group(1) if name else "mesh", "positions": positions, "normals": normals, "uvs": uvs, "submeshes": subs}


# ---------------------------------------------------------------- transforms
def quat_to_mat(q):
    x, y, z, w = q
    return [
        [1 - 2 * (y * y + z * z), 2 * (x * y - z * w), 2 * (x * z + y * w)],
        [2 * (x * y + z * w), 1 - 2 * (x * x + z * z), 2 * (y * z - x * w)],
        [2 * (x * z - y * w), 2 * (y * z + x * w), 1 - 2 * (x * x + y * y)],
    ]


def trs(p, q, s):
    r = quat_to_mat(q)
    return [
        [r[0][0] * s[0], r[0][1] * s[1], r[0][2] * s[2], p[0]],
        [r[1][0] * s[0], r[1][1] * s[1], r[1][2] * s[2], p[1]],
        [r[2][0] * s[0], r[2][1] * s[1], r[2][2] * s[2], p[2]],
        [0, 0, 0, 1],
    ]


def mul(a, b):
    return [[sum(a[i][k] * b[k][j] for k in range(4)) for j in range(4)] for i in range(4)]


IDENT = [[1, 0, 0, 0], [0, 1, 0, 0], [0, 0, 1, 0], [0, 0, 0, 1]]


class Prefab:
    """GameObjects of one prefab with their transforms (relative to the root) and components."""

    def __init__(self, pkg, guid):
        self.pkg = pkg
        self.docs = pkg.docs(guid)
        self.go = {}
        self.tr = {}
        self.comp_by_go = {}
        for fid, (cls, kind, f) in self.docs.items():
            if kind == "GameObject":
                self.go[fid] = f
            elif kind in ("Transform", "RectTransform"):
                self.tr[fid] = f
            go = (f.get("m_GameObject") or {}).get("fileID") if isinstance(f, dict) else None
            if go:
                self.comp_by_go.setdefault(go, []).append((fid, kind, f))
        self.root = next(fid for fid, f in self.tr.items() if not (f.get("m_Father") or {}).get("fileID"))
        self.nodes = []  # (path, depth, transform fid, go fid, local matrix, world matrix, active, activeInHierarchy)
        self._walk(self.root, "", 0, IDENT, True, True)

    def name_of(self, tfid):
        return self.go[self.tr[tfid]["m_GameObject"]["fileID"]].get("m_Name", "?")

    def _walk(self, tfid, parent_path, depth, parent_world, parent_active, is_root):
        f = self.tr[tfid]
        gofid = f["m_GameObject"]["fileID"]
        g = self.go[gofid]
        name = str(g.get("m_Name", "?"))
        p = f.get("m_LocalPosition", {})
        q = f.get("m_LocalRotation", {})
        s = f.get("m_LocalScale", {})
        local = trs((p.get("x", 0), p.get("y", 0), p.get("z", 0)), (q.get("x", 0), q.get("y", 0), q.get("z", 0), q.get("w", 1)), (s.get("x", 1), s.get("y", 1), s.get("z", 1)))
        world = IDENT if is_root else mul(parent_world, local)
        active = bool(g.get("m_IsActive", 1))
        path = "" if is_root else (parent_path + "/" + name if parent_path else name)
        self.nodes.append({"path": path, "name": name, "depth": depth, "t": tfid, "go": gofid, "world": world, "active": active, "inHierarchy": parent_active and active, "layer": g.get("m_Layer"), "tag": g.get("m_TagString")})
        for c in f.get("m_Children") or []:
            self._walk(c["fileID"], path, depth + 1, world, parent_active and active, False)

    def components(self, gofid):
        return self.comp_by_go.get(gofid, [])

    def script_name(self, f):
        s = (f.get("m_Script") or {}).get("guid")
        p = self.pkg.path(s) if s else None
        return os.path.splitext(os.path.basename(p))[0] if p else "MonoBehaviour"
