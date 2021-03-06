using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ２次元多角形を三角形に分割するクラス
/// This script can be used to split a 2D polygon into triangles.
/// The algorithm supports concave polygons,
/// but not polygons with holes, or multiple polygons at once.    
/// http://www.unifycommunity.com/wiki/index.php?title=Triangulator
/// 
/// 外側から三角形をそぎ落としていく感じのアルゴリズム.
/// </summary>
/// 
public static class Triangulator
{
	public static List<int> Triangulate (Vector2[] inPoints)
	{
		List<int> indices = new List<int> ();
        
		int n = inPoints.Length;
		if (n < 3)
			return indices;

		int[] V = new int[n];
		if (Area (inPoints) > 0) {
			for (int v = 0; v < n; v++)
				V [v] = v;
		} else {
			for (int v = 0; v < n; v++)
				V [v] = (n - 1) - v;
		}
        
		int nv = n;
		int count = 2 * nv;
		for (int m = 0, v = nv - 1; nv > 2;) {
			if ((count--) <= 0)
				return indices;
            
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
            
			if (Snip (inPoints, u, v, w, nv, V)) {
				int a, b, c, s, t;
				a = V [u];
				b = V [v];
				c = V [w];
				indices.Add (a);
				indices.Add (b);
				indices.Add (c);
				m++;
				for (s = v, t = v + 1; t < nv; s++, t++)
					V [s] = V [t];
				nv--;
				count = 2 * nv;
			}
		}
		indices.Reverse ();
		return indices;
	}
	
    static float Area (Vector2[] m_points) {
        int n = m_points.Length;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }
    
    static bool Snip (Vector2[] m_points, int u, int v, int w, int n, int[] V) {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++) {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }
	
    static bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;
        
        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;
        
        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;
        
        return ((aCROSSbp > 0.0f) && (bCROSScp > 0.0f) && (cCROSSap > 0.0f));
    }
}