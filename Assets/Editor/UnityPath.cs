using UnityEngine;
using System.Collections;

/// <summary>
/// 場面によってパスがいろいろなので変換する。
/// </summary>
public static class UnityPath {
	/// <summary>
	/// Unity Projectのディレクトリフルパス
	/// </summary>
	/// <returns>
	/// パス
	/// </returns>
	public static string GetProjectFullPath ()
	{
		return Application.dataPath.Substring (0, Application.dataPath.Length - 7);
	}
	
	/// <summary>
	/// 完全なパスをAssetDatabaseを扱う際のパスに変換する
	/// </summary>
	/// <returns>
	/// 完全なパス
	/// </returns>
	/// <param name='path'>
	/// AssetDatabaseを扱う際のパス
	/// </param>
	public static string FullToAssetDatabasePath (string path)
	{
		return path.Replace (GetProjectFullPath () + "/", string.Empty);
	}
	
	/// <summary>
	/// AssetDatabaseを扱う際のパスを完全なパスに変換する
	/// </summary>
	/// <returns>
	/// AssetDatabaseを扱う際のパス
	/// </returns>
	/// <param name='path'>
	/// 完全なパス
	/// </param>
	public static string AssetDatabaseToFullPath (string path)
	{
		if (path.IndexOf ('/') == 0) {
			return GetProjectFullPath () + path;
		}
		return GetProjectFullPath () + "/" + path;
	}
}
