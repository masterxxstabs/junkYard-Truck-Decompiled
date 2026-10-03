"""Render quadzilla.obj (StockEngine hidden) together with the engine exported
from fit mode (F6), to check the fit outside the game.

Usage (in the folder with quadzilla.obj/.mtl):
  python3 render_fit.py engine250_export.obj [out.png]   (needs numpy, matplotlib)
"""
import sys, numpy as np, matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from matplotlib.collections import PolyCollection
def load(obj, mtl, skip=()):
    col={}; cur=None
    try:
        for l in open(mtl):
            p=l.split()
            if p and p[0]=='newmtl': cur=p[1]
            if p and p[0]=='Kd': col[cur]=np.array(list(map(float,p[1:4])))
    except FileNotFoundError: pass
    V=[];F=[];C=[];O=[];mat=None;o=None
    for l in open(obj):
        if l.startswith('v '): V.append(list(map(float,l.split()[1:4])))
        elif l.startswith('o '): o=l.split()[1]
        elif l.startswith('usemtl'): mat=l.split()[1]
        elif l.startswith('f '):
            idx=[int(t.split('/')[0])-1 for t in l.split()[1:]]
            for k in range(1,len(idx)-1):
                F.append([idx[0],idx[k],idx[k+1]]); C.append(col.get(mat,np.array([.5,.5,.5]))); O.append(o)
    F=np.array(F);C=np.array(C);O=np.array(O)
    keep=~np.isin(O,list(skip))
    return np.array(V),F[keep],C[keep]
qv,qf,qc=load('quadzilla.obj','quadzilla.mtl',skip=('StockEngine',))
ev,ef,ec=load(sys.argv[1],sys.argv[1][:-4]+'.mtl')
ec=np.where(ec.sum(1,keepdims=True)>0,ec,.5)
V=np.vstack([qv,ev]); F=np.vstack([qf,ef+len(qv)]); C=np.vstack([qc,np.clip(ec*0.6+np.array([.4,.05,.05]),0,1) if '--tint' in sys.argv else ec])
isE=np.r_[np.zeros(len(qf),bool),np.ones(len(ef),bool)]
V=V*np.array([-1,1,1])
lo,hi=ev.min(0),ev.max(0)
print('engine size (w,h,l) m:',np.round(hi-lo,3),' bottom y',round(lo[1],3))
def render(ax, yaw, pitch, title, mask, zoom=None):
    cy,sy=np.cos(np.radians(yaw)),np.sin(np.radians(yaw)); cp,sp=np.cos(np.radians(pitch)),np.sin(np.radians(pitch))
    Ry=np.array([[cy,0,sy],[0,1,0],[-sy,0,cy]]); Rx=np.array([[1,0,0],[0,cp,-sp],[0,sp,cp]])
    P=V@(Rx@Ry).T; ff=F[mask]; cc=C[mask]
    tri=P[ff]; n=np.cross(tri[:,1]-tri[:,0],tri[:,2]-tri[:,0]); n/=np.linalg.norm(n,axis=1,keepdims=True)+1e-12
    light=np.array([0.4,0.7,-0.6]); light/=np.linalg.norm(light)
    shade=0.35+0.65*np.abs(n@light)
    order=np.argsort(-tri[:,:,2].mean(axis=1))
    ax.add_collection(PolyCollection(tri[order][:,:,:2],facecolors=np.clip(cc[order]*shade[order,None],0,1),edgecolors='none'))
    Pm=P[np.unique(ff)]
    if zoom: ax.set_xlim(*zoom[0]); ax.set_ylim(*zoom[1])
    else: ax.set_xlim(Pm[:,0].min()-.05,Pm[:,0].max()+.05); ax.set_ylim(Pm[:,1].min()-.05,Pm[:,1].max()+.05)
    ax.set_aspect('equal'); ax.axis('off'); ax.set_title(title,fontsize=15)
allm=np.ones(len(F),bool)
fig,ax=plt.subplots(2,2,figsize=(18,14),facecolor='#dfe6ec')
render(ax[0,0],90,-5,'Side (left)',allm,zoom=((-0.6,0.6),(0.05,0.9)))
render(ax[0,1],-90,-5,'Side (right)',allm,zoom=((-0.6,0.6),(0.05,0.9)))
render(ax[1,0],215,-25,'Front three-quarter',allm)
render(ax[1,1],0,-90,'From above, bodywork and wheels as is',allm,zoom=((-0.45,0.45),(-0.6,0.6)))
plt.tight_layout(); plt.savefig(sys.argv[2] if len(sys.argv)>2 and not sys.argv[2].startswith('--') else 'engine_fit.png',dpi=60,facecolor=fig.get_facecolor())
