using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    int[,] map; // レベルデザイン用の配列
    GameObject[,] field; // ゲーム管理用の配列
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject particlePrefab;
    public GameObject wallPrefab;
    public GameObject clearText;

    // クラスの中の、メソッドの外に書くことに注意
    // 返り値の型に注意

    /// <summary> 
    /// プレイヤーのインデックスを取得
    /// </summary>
    /// <returns></returns>
    Vector2Int GetPlayerIndex()
    {
        //要素数はmap.Lengthで取得
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player") { return new Vector2Int(x, y); }
            }
        }
        // 見つからなかった時のために-1を返す
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// num配列の数字を移動させる
    /// </summary>
    /// <param name="tag">動かす物のタグ</param>
    /// <param name="moveFrom">動かす元のインデックス</param>
    /// <param name="moveTo">動かす先のインデックス</param>
    /// <returns></returns>
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        // 移動先が範囲外なら移動不可
        // 動けない条件を先に書き、リターンする。早期リターン
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        // 移動先に4(壁)が居たら
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            return false;
        }

        // 移動先に2(箱)が居たら
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            // どの方向に移動するかを算出
            Vector2Int velocity = moveTo - moveFrom;
            // プレイヤーの移動先から、さらに先へ2(箱)を移動させる。
            // 箱の移動処理。MoveNumberメソッド内でMoveMenberメソッドを
            // 呼び、処理が再起している。移動可不可をboolで記録
            bool success = MoveNumber("Box", moveTo, moveTo + velocity);
            // もし箱が移動失敗したら、プレイヤーの移動も失敗
            if (!success) { return false; }
        }

        // プレイヤー・箱関わらずの移動処理
        // field[moveFrom.y, moveFrom.x].transform.position = IndexToPosition(moveTo);

        Vector3 moveToPosition = IndexToPosition(moveTo);
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        // パーティクル呼び出し
        for (int i = 0; i < 4; ++i)
        {
            GameObject particle = Instantiate(
                particlePrefab,
                IndexToPosition(moveFrom),
                Quaternion.identity
                );
        }

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(
            index.x - map.GetLength(1) / 2 + 0.5f,
            index.y - map.GetLength(0) / 2,
            0);
    }

    /// <summary>
    /// クリア判定を行う
    /// </summary>
    /// <returns></returns>
    bool IsCleard()
    {
        // Vector2Int型の可変長配列
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // 格納場所か否かを判断
                if (map[y, x] == 3)
                {
                    // 格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        // 要素数はgoals.Countで取得
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // 一つでも箱が無かったら条件未達成
                return false;
            }
        }

        // 条件未達成でなければ条件達成
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        // 配列の実態の作成と初期化
        map = new int[,] { // 8*7サイズ
            { 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 0, 0, 0, 0, 0, 0, 4 },
            { 4, 0, 0, 0, 2, 3, 0, 4 },
            { 4, 0, 1, 0, 2, 3, 0, 4 },
            { 4, 0, 0, 0, 2, 3, 0, 4 },
            { 4, 0, 0, 0, 0, 0, 0, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4 },
        };

        field = new GameObject[map.GetLength(0), map.GetLength(1)];

        // 変更。二重for文で二次元配列の情報を出力
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }

                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }

                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                        goalPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }

                if (map[y, x] == 4)
                {
                    field[y, x] = Instantiate(
                        wallPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // メソッド化した処理を使用
            Vector2Int playerIndex = GetPlayerIndex();

            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));

            // ゲームオブジェクトのSetActiveメソッドを使い有効化
            clearText.SetActive(IsCleard());
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // メソッド化した処理を使用
            Vector2Int playerIndex = GetPlayerIndex();

            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // メソッド化した処理を使用
            Vector2Int playerIndex = GetPlayerIndex();

            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // メソッド化した処理を使用
            Vector2Int playerIndex = GetPlayerIndex();

            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
        }

        // リセット処理
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Destroy(field[y, x]);

                    if (map[y, x] == 1)
                    {
                        field[y, x] = Instantiate(
                            playerPrefab,
                            IndexToPosition(new Vector2Int(x, y)),
                            Quaternion.identity
                            );
                    }

                    if (map[y, x] == 2)
                    {
                        field[y, x] = Instantiate(
                            boxPrefab,
                            IndexToPosition(new Vector2Int(x, y)),
                            Quaternion.identity
                            );
                    }

                    if (map[y, x] == 3)
                    {
                        field[y, x] = Instantiate(
                            goalPrefab,
                            IndexToPosition(new Vector2Int(x, y)),
                            Quaternion.identity
                            );
                    }

                    if (map[y, x] == 4)
                    {
                        field[y, x] = Instantiate(
                            wallPrefab,
                            IndexToPosition(new Vector2Int(x, y)),
                            Quaternion.identity
                            );
                    }
                }
            }
        }

        // ゲームオブジェクトのSetActiveメソッドを使い有効化
        clearText.SetActive(IsCleard());
    }
}
