using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// A-star algorithm
public class AStar : MonoBehaviour {

	struct Point2 {
		public int x;
		public int z;
		public Point2(int x=0, int z=0) {
			this.x = x;
			this.z = z;
		}

		public void Set(int x, int z) {
			this.x = x;
			this.z = z;
		}
	}

	/// A-starノード.
	class ANode {
		enum eStatus {
			None,
			Open,
			Closed,
		}
		/// ステータス
		eStatus _status = eStatus.None;
		/// 実コスト
		int _cost = 0;
		/// ヒューリスティック・コスト
		int _heuristic = 0;
		/// 親ノード
		ANode _parent = null;
		/// 座標
		int _x = 0;
		int _z = 0;
		public int X {
			get { return _x; }
		}
		public int Z {
			get { return _z; }
		}
		public int Cost {
			get { return _cost; }
		}

		/// コンストラクタ.
		public ANode(int x, int z) {
			_x = x;
			_z = z;
		}
		/// スコアを計算する.
		public int GetScore() {
			return _cost + _heuristic;
		}
		/// ヒューリスティック・コストの計算.
		public void CalcHeuristic(int xgoal, int zgoal) {
            
			// 縦横移動のみ
			var dx = Mathf.Abs (xgoal - X);
			var dz = Mathf.Abs (zgoal - Z);
			_heuristic = (int)(dx + dz);
		}
		/// ステータスがNoneかどうか.
		public bool IsNone() {
			return _status == eStatus.None;
		}
		/// ステータスをOpenにする.
		public void Open(ANode parent, int cost) {
			_status = eStatus.Open;
			_cost   = cost;
			_parent = parent;
		}
		/// ステータスをClosedにする.
		public void Close() {
			_status = eStatus.Closed;
		}
		/// パスを取得する
		public void GetPath(List<Point2> pList) {
			pList.Add(new Point2(X, Z));
			if(_parent != null) {
				_parent.GetPath(pList);
			}
		}
	}

	/// A-starノード管理.
	class ANodeMgr
    {
        /// 地形レイヤー.
        Layer2D _layer;
		/// オープンリスト.
		List<ANode> _openList = null;
		/// ノードインスタンス管理.
		Dictionary<int,ANode> _pool = null;
		/// ゴール座標.
		int _xgoal = 0;
		int _zgoal = 0;
        /// オブジェクトネーム
        string _name;
		public ANodeMgr(string name, Layer2D layer, int xgoal, int zgoal) {
            _layer = layer;
            _openList = new List<ANode>();
			_pool = new Dictionary<int, ANode>();
			_xgoal = xgoal;
			_zgoal = zgoal;
            _name = name;

        }
		/// ノード生成する.
		public ANode GetNode(int x, int z) {
			var idx = _layer.ToIdx(x, z);
			if(_pool.ContainsKey(idx)) {
				// 既に存在しているのでプーリングから取得.
				return _pool[idx];
			}

			// ないので新規作成.
			var node = new ANode(x, z);
			_pool[idx] = node;
			// ヒューリスティック・コストを計算する.
			node.CalcHeuristic(_xgoal, _zgoal);
			return node;
		}
		/// ノードをオープンリストに追加する.
		public void AddOpenList(ANode node) {
			_openList.Add(node);
		}
		/// ノードをオープンリストから削除する.
		public void RemoveOpenList(ANode node) {
			_openList.Remove(node);
		}
		/// 指定の座標にあるノードをオープンする.
		public ANode OpenNode(int x, int z, int cost, ANode parent) {
			// 座標をチェック.
			if(_layer.IsOutOfRange(x, z)) {
				// 領域外.
				return null;
			}
			if(_layer.Get(x, z)) {
				// 通過できない.
				return null;
			}
            // SandCheck
            if(_layer.SandCheck(x, z, _name[_name.IndexOf("Player") - 1].ToString()))
            {
                // 通過できない
                return null;
            }
			// ノードを取得する.
			var node = GetNode(x, z);
			if(node.IsNone() == false) {
				// 既にOpenしているので何もしない
				return null;
			}

			// Openする.
			node.Open(parent, cost);
			AddOpenList(node);

			return node;
		}

		/// 周りをOpenする.
		public void OpenAround(ANode parent) {
			var xbase = parent.X; // 基準座標(X).
			var zbase = parent.Z; // 基準座標(Y).
			var cost = parent.Cost; // コスト.
			cost += 1; // 一歩進むので+1する.
			
            // 4方向を開く.
			var x = xbase;
			var z = zbase;
			OpenNode (x-1, z,   cost, parent); // 右.
			OpenNode (x,   z-1, cost, parent); // 上.
			OpenNode (x+1, z,   cost, parent); // 左.
			OpenNode (x,   z+1, cost, parent); // 下.
		}

		/// 最小スコアのノードを取得する.
		public ANode SearchMinScoreNodeFromOpenList() {
			// 最小スコア
			int min = 9999;
			// 最小実コスト
			int minCost = 9999;
			ANode minNode = null;
			foreach(ANode node in _openList) {
				int score = node.GetScore();
				if(score > min) {
					// スコアが大きい
					continue;
				}
				if(score == min && node.Cost >= minCost) {
					// スコアが同じときは実コストも比較する
					continue;
				}

				// 最小値更新.
				min = score;
				minCost = node.Cost;
				minNode = node;
			}
			return minNode;
		}
	}

	/// ランダムな座標を取得する.
	Point2 GetRandomPosition(Layer2D layer) {
		Point2 p;
		while(true) {
			p.x = Random.Range(0, layer.Width);
			p.z = Random.Range(0, layer.Height);
			if(!layer.Get(p.x, p.z)) {
				// 通過可能
				break;
			}
		}
		return p;
	}

	// 状態.
	enum eState {
		Exec, // 実行中.
		Walk, // 移動中.
		End,  // おしまい.
	}
	eState _state = eState.Exec;
    FieldObjectBase _fieldObjBase;
    Layer2D _layer;
    List<int> _RouteList = new List<int>();
    public List<int> GetRoute { get { return _RouteList; } }

    void Awake()
    {
        _fieldObjBase = GetComponent<FieldObjectBase>();

        // 地形データのロード.
        _layer = new Layer2D();
        _layer.Create(this.gameObject);
    }

	public void Search (int nTargetNumber)
    {
        List<Point2> pList = new List<Point2>();
        GameObject player = this.gameObject;
        _RouteList.Clear();
        // A-star実行.
        {
            // スタート地点.
            Point2 pStart = new Point2();   //  場所指定
            int number = _fieldObjBase.GetDataNumber();
            pStart.x = number % GameScaler._nWidth;
            pStart.z = number / GameScaler._nWidth;
            //Debug.Log("自分の位置 " + number +  ", x : " + pStart.x + ", z : " + pStart.z);

            // ゴール. /* ここから */
            Point2 pGoal = new Point2();    //  場所指定
            pGoal.x = nTargetNumber % GameScaler._nWidth;
            pGoal.z = nTargetNumber / GameScaler._nWidth;
            //Debug.Log("行先の位置 " + nTargetNumber + ", x : " + pGoal.x + ", z : " + pGoal.z);

            var mgr = new ANodeMgr(this.name, _layer, pGoal.x, pGoal.z);

			// スタート地点のノード取得
			// スタート地点なのでコストは「0」
			ANode node = mgr.OpenNode(pStart.x, pStart.z, 0, null);
			mgr.AddOpenList(node);

			// 試行回数。100回超えたら強制中断
			int cnt = 0;
            while (cnt < 100)
            {
                mgr.RemoveOpenList(node);
                // 周囲を開く
                mgr.OpenAround(node);
                // 最小スコアのノードを探す.
                node = mgr.SearchMinScoreNodeFromOpenList();
                if (node == null)
                {
                    break;
                }
                if (node.X == pGoal.x && node.Z == pGoal.z)
                {
                    // ゴールにたどり着いた.
                    mgr.RemoveOpenList(node);
                    // パスを取得する
                    node.GetPath(pList);
                    // 反転する
                    pList.Reverse();
                    break;
                }
                cnt++;
            }
            Debug.Log("経路探索回数 : " + cnt);
		}

        //  ルートに変換して実行
        foreach (Point2 point in pList)
        {
            _RouteList.Add(_layer.ToIdx(point.x, point.z));
        }
    }
}
