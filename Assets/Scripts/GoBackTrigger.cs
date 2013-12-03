using UnityEngine;
using System.Collections;

/// <summary>
/// トリガーに入ったボールをを元の位置に戻す。
/// </summary>
public class GoBackTrigger : MonoBehaviour {
	
	/// <summary>
	/// 元の位置のオブジェクト。
	/// </summary>
	public GameObject target;
	
	/// <summary>
	/// GameObject.TriggerEnter
	/// ボールを元の位置に戻す。
	/// </summary>
	/// <param name='collider'>
	/// Collider.
	/// </param>
	void OnTriggerEnter (Collider collider)
	{
		print ("OnTriggerEnter");
		// Project Settings/Physicsで当たり判定のレイヤーを設定しているので、ここでオブジェクトを判別する必要はない。
		// Meshのサイズを1x1x1にしておくと、以下のように座標計算が便利。
		collider.transform.position = target.transform.position
			+ new Vector3 (0, (collider.transform.localScale.y + target.transform.localScale.y) / 2, 0);
		
		// x軸を右向きにしておくとVector3.left, rightの向きと合う。
		collider.rigidbody.velocity = Vector3.right;
	}
}