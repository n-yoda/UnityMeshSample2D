using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// メッシュ生成のサンプル。
/// </summary>
public class GenerateSamples : EditorWindow {
	
	/// <summary>
	/// 三角形とそれ用のCollider用Meshを作る。
	/// </summary>
	[MenuItem("Generate Mesh/Triangle Set")]
	static void GanerateTriangle ()
	{
		Vector2[] polygon = {  new Vector2 (0, 0), new Vector2 (0, 1), new Vector2 (1, 0),};
		string savePath = UnityPath.FullToAssetDatabasePath (EditorUtility.SaveFilePanel ("Save Mesh", Application.dataPath, "Triangle", "asset"));
		string formatPath = savePath.Replace (".asset", "{0}.asset");
		AssetDatabase.CreateAsset (MeshGenerater.Polygon2D (polygon, Vector3.back, Vector3.up), string.Format (formatPath, "Plane"));
		AssetDatabase.CreateAsset (MeshGenerater.PillarForCollider (polygon, Vector3.back, Vector3.up), string.Format (formatPath, "Collider"));
		AssetDatabase.Refresh ();
	}
	
	/// <summary>
	/// 4方向のPlaneを作成
	/// </summary>
	[MenuItem("Generate Mesh/Planes")]
	static void GaneratePlanes ()
	{
		string savePath = UnityPath.FullToAssetDatabasePath (EditorUtility.SaveFilePanel ("Save Mesh", Application.dataPath, "Plane", "asset"));
		string formatPath = savePath.Replace (".asset", "{0}.asset");
		AssetDatabase.CreateAsset (MeshGenerater.UnitPlane (Vector3.back, Vector3.up), string.Format (formatPath, "-Z"));
		AssetDatabase.CreateAsset (MeshGenerater.UnitPlane (Vector3.forward, Vector3.up), string.Format (formatPath, "+Z"));
		AssetDatabase.CreateAsset (MeshGenerater.UnitPlane (Vector3.right, Vector3.up), string.Format (formatPath, "+X"));
		AssetDatabase.CreateAsset (MeshGenerater.UnitPlane (Vector3.left, Vector3.up), string.Format (formatPath, "-X"));
		AssetDatabase.CreateAsset (MeshGenerater.UnitPlane (Vector3.down, Vector3.back), string.Format (formatPath, "-Y"));
		AssetDatabase.CreateAsset (MeshGenerater.UnitPlane (Vector3.up, Vector3.forward), string.Format (formatPath, "+Y"));
		AssetDatabase.Refresh ();
	}
	
	/// <summary>
	/// 4方向の円を作成
	/// </summary>
	[MenuItem("Generate Mesh/Circles")]
	static void GanerateCircles ()
	{
		//分割数
		const int partition = 24;
		string savePath = UnityPath.FullToAssetDatabasePath (EditorUtility.SaveFilePanel ("Save Mesh", Application.dataPath, "Circle", "asset"));
		string formatPath = savePath.Replace (".asset", "{0}.asset");
		AssetDatabase.CreateAsset (MeshGenerater.Circle (Vector3.back, Vector3.up, partition), string.Format (formatPath, "-Z"));
		//AssetDatabase.CreateAsset (MeshGenerater.Circle (Vector3.forward, Vector3.up, partition), string.Format (formatPath, "+Z"));
		//AssetDatabase.CreateAsset (MeshGenerater.Circle (Vector3.right, Vector3.up, partition), string.Format (formatPath, "+X"));
		//AssetDatabase.CreateAsset (MeshGenerater.Circle (Vector3.left, Vector3.up, partition), string.Format (formatPath, "-X"));
		AssetDatabase.Refresh ();
	}
	
	/// <summary>
	/// Z方向の円錐
	/// </summary>
	[MenuItem("Generate Mesh/Cylinder")]
	static void GanerateCylinder ()
	{
		//分割数
		const int partition = 24;
		float unitRad = (Mathf.PI * 2) / partition;
		Vector2[] poly = Enumerable.Range (0, partition).Select (i => new Vector2 (Mathf.Sin (unitRad * i), Mathf.Cos (unitRad * i))).ToArray ();
		string savePath = UnityPath.FullToAssetDatabasePath (EditorUtility.SaveFilePanel ("Save Mesh", Application.dataPath, "Cylinder", "asset"));
		string formatPath = savePath.Replace (".asset", "{0}.asset");
		AssetDatabase.CreateAsset (MeshGenerater.Pillar (poly, Vector3.back, Vector3.up), string.Format (formatPath, "-Z"));
		//シリンダー型のColliderを使うくらいならSphereColliderを使ったほうが良い。
		//AssetDatabase.CreateAsset (MeshGenerater.PillarForCollider (poly, Vector3.back, Vector3.up), string.Format (formatPath, "Collider"));
		AssetDatabase.Refresh ();
	}
}
