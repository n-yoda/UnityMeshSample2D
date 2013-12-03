using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Meshデータを生成する。
/// </summary>
public static class MeshGenerater {
	/// <summary>
	/// 1x1サイズの単位正方形を生成する。
	/// 向きと上方向を指定できる。
	/// </summary>
	/// <returns>
	/// Meshオブジェクト。
	/// </returns>
	/// <param name='normal'>
	/// 面の向きを指すベクトル。
	/// </param>
	/// <param name='upwards'>
	/// 上向き方向を指すベクトル。
	/// </param>
	public static Mesh UnitPlane (Vector3 normal, Vector3 upwards)
	{
		float half = 0.5f;
		upwards = upwards.normalized * half;
		Vector3 right = Vector3.Cross (normal, upwards).normalized * half;
		Vector3[] vertex = new Vector3[]{ 
			-upwards - right,
			upwards - right,
			upwards + right,
			-upwards + right,
		};
		int[] triangles = new int[]{
			0,1,2,
			2,3,0
		};
		Vector2[] uv = new Vector2[]{
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2 (1, 1),
			new Vector2 (1, 0),
		};
		Vector3[] normals = new Vector3[]{
			normal, normal, normal, normal
		};
		
		Mesh mesh = new Mesh ();
		mesh.vertices = vertex;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.normals = normals;
		mesh.RecalculateBounds ();
		return mesh;
		
		//遠回りだけど↓でもOK.
		//return Polygon2D (new Vector2[]{new Vector2 (0, 0),new Vector2 (0, 1),new Vector2 (1, 1),new Vector2 (1, 0)}, normal, upwards);
	}
	
	/// <summary>
	/// 1x1サイズの円を生成する。
	/// 向きと上方向と分割数を指定できる。
	/// （わかりやすく扇型分割をしているのでポリゴン数は最少よりちょっと多い。）
	/// </summary>
	/// <returns>
	/// Meshオブジェクト。
	/// </returns>
	/// <param name='normal'>
	/// 面の向きを指すベクトル。
	/// </param>
	/// <param name='upwards'>
	/// 上向き方向を指すベクトル。
	/// </param>
	public static Mesh Circle (Vector3 normal, Vector3 upwards, int partition)
	{
		float half = 0.5f;
		upwards = upwards.normalized * half;
		Vector3 right = Vector3.Cross (normal, upwards).normalized * half;
		float unitRad = (Mathf.PI * 2) / partition;
		
		//上から時計回りに頂点を作る。
		List<Vector3> vertices =
			Enumerable.Range (0, partition).Select (i => upwards * Mathf.Cos (unitRad * i) + right * Mathf.Sin (unitRad * i)).ToList ();
		
		//uvも同様
		List<Vector2> uv = Enumerable.Range (0, partition).Select (i => new Vector2 (Mathf.Sin (unitRad * i) * half + half, Mathf.Cos (unitRad * i) * half + half)).ToList ();
		
		//三角形を作っていく
		int[] triangles = Enumerable.Range (0, partition).SelectMany (i => new List<int> (){i, (i + 1) % vertices.Count, vertices.Count}).ToArray ();
		
		//上のループが楽だから敢えてあとから追加
		vertices.Add (Vector3.zero);
		uv.Add (new Vector2(half, half));
		
		//全部同じ向き
		Vector3[] normals = Enumerable.Repeat<Vector3> (normal, vertices.Count).ToArray ();
		
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles;
		mesh.uv = uv.ToArray ();
		mesh.normals = normals;
		mesh.RecalculateBounds ();
		return mesh;
	}
	
	/// <summary>
	/// 任意の多角形を柱状にする。
	/// ただし、1x1x1に収まるように変形される。（その方が便利だと思う）
	/// </summary>
	/// <param name='polygon'>
	/// ２次元平面上の多角形。
	/// 左下を原点とする座標系。
	/// </param>
	/// <param name='direction'>
	/// Direction.
	/// </param>
	public static Mesh Pillar (Vector2[] polygon, Vector3 normal, Vector3 upwards)
	{
		float half = 0.5f;
		Vector3 yUnit = upwards.normalized;
		Vector3 xUnit = Vector3.Cross (normal, upwards).normalized;
		Vector3 zUnit = normal.normalized;
		
		//表面、裏面、側面のための表面、側面のための裏面
		IEnumerable<Vector2> uv = StretchPolygonToRect (polygon, new Rect (0, 0, 1, 1));
		uv = uv.Concat (uv);
		uv = uv.Concat (Enumerable.Range (0, polygon.Length).Select (i => new Vector2 ((1f / polygon.Length) * i, 0)));		
		uv = uv.Concat (Enumerable.Range (0, polygon.Length).Select (i => new Vector2 ((1f / polygon.Length) * i, 1)));
		
		Vector2[] pos = StretchPolygonToRect (polygon, new Rect (-half, -half, 1, 1));
		IEnumerable<Vector3> vertices = pos.Select (p => p.x * xUnit + p.y * yUnit + zUnit * half);
		vertices = vertices.Concat (vertices.Select (v => v - zUnit));
		vertices = vertices.Concat (vertices);
		
		int[] triangles = Triangulator.Triangulate (polygon).ToArray ();
		int[] triangles2 = Triangulator.Triangulate (polygon).ToArray ();
		System.Array.Reverse (triangles2);
		
		IEnumerable<int> triangles3 = Enumerable.Range (polygon.Length * 2, polygon.Length)
			.SelectMany (i => {
			int next = (i + 1) % polygon.Length + polygon.Length * 2;
			return new List<int> (){i, i + polygon.Length, next + polygon.Length, i, next + polygon.Length, next};
		});
		
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.uv = uv.ToArray ();
		mesh.triangles = triangles.Concat (triangles2.Select (i => i + polygon.Length)).Concat (triangles3).ToArray ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		return mesh;
	}
	
	
	/// <summary>
	/// 任意の多角形を柱状にするが、底面を含めない。
	/// 当たり判定専用！
	/// ただし、1x1x1に収まるように変形される。（その方が便利だと思う）
	/// </summary>
	/// <param name='polygon'>
	/// ２次元平面上の多角形。
	/// 左下を原点とする座標系。
	/// </param>
	/// <param name='direction'>
	/// Direction.
	/// </param>
	public static Mesh PillarForCollider (Vector2[] polygon, Vector3 normal, Vector3 upwards)
	{
		float half = 0.5f;
		Vector3 yUnit = upwards.normalized;
		Vector3 xUnit = Vector3.Cross (normal, upwards).normalized;
		Vector3 zUnit = normal.normalized;
		
		Vector2[] pos = StretchPolygonToRect (polygon, new Rect (-half, -half, 1, 1));
		IEnumerable<Vector3> vertices = pos.Select (p => p.x * xUnit + p.y * yUnit + zUnit * half);
		vertices = vertices.Concat (vertices.Select (v => v - zUnit));

		IEnumerable<int> triangles = Enumerable.Range (0, polygon.Length)
			.SelectMany (i => {
			int next = (i + 1) % polygon.Length;
			return new List<int> (){i, i + polygon.Length, next + polygon.Length, i, next + polygon.Length, next};
		});
		
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		return mesh;
	}
	
	/// <summary>
	/// 任意の多角形を☄特定方向を向いた面Meshにする。
	/// 主に当たり判定に使う。
	/// ただし、1x1x1に収まるように変形される。（その方が便利だと思う）
	/// </summary>
	/// <param name='polygon'>
	/// ２次元平面上の多角形。
	/// 左下を原点とする座標系。
	/// </param>
	/// <param name='direction'>
	/// Direction.
	/// </param>
	public static Mesh Polygon2D (Vector2[] polygon, Vector3 normal, Vector3 upwards)
	{
		float half = 0.5f;
		Vector3 yUnit = upwards.normalized;
		Vector3 xUnit = Vector3.Cross (normal, upwards).normalized;
		Vector2[] uv = StretchPolygonToRect (polygon, new Rect (0, 0, 1, 1));
		Vector2[] pos = StretchPolygonToRect (polygon, new Rect (-half, -half, 1, 1));
		Vector3[] vertices = pos.Select (p => p.x * xUnit + p.y * yUnit).ToArray ();
		Vector3[] normals = Enumerable.Repeat (normal, polygon.Length).ToArray ();
		int[] triangles = Triangulator.Triangulate (polygon).ToArray ();
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.normals = normals;
		mesh.RecalculateBounds ();
		return mesh;
	}
	
	/// <summary>
	/// 多角形をRectの中に収まるようにリサイズする。
	/// </summary>
	/// <returns>
	/// リサイズされたポリゴン。
	/// </returns>
	/// <param name='inPolygon'>
	/// もとのポリゴン。
	/// </param>
	/// <param name='inRect'>
	/// 収めたい四角形
	/// </param>
	static Vector2[] StretchPolygonToRect (Vector2[] inPolygon, Rect inRect)
	{
		float xMin = inPolygon.Select (vec => vec.x).Min ();
		float yMin = inPolygon.Select (vec => vec.y).Min ();
		float xMax = inPolygon.Select (vec => vec.x).Max ();
		float yMax = inPolygon.Select (vec => vec.y).Max ();
		Vector2 scaler = new Vector2 (inRect.width / (xMax - xMin), inRect.height / (yMax - yMin));
		Vector2 origin = new Vector2 (inRect.x, inRect.y);
		
		return inPolygon.Select(vec => origin + new Vector2((vec.x - xMin) * scaler.x, (vec.y - yMin) * scaler.y)).ToArray();
	}
}
