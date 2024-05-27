using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    int[,] map; // ���x���f�U�C���p�̔z��
    GameObject[,] field; // �Q�[���Ǘ��p�̔z��
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject particlePrefab;
    public GameObject wallPrefab;
    public GameObject clearText;

    // �N���X�̒��́A���\�b�h�̊O�ɏ������Ƃɒ���
    // �Ԃ�l�̌^�ɒ���

    /// <summary> 
    /// �v���C���[�̃C���f�b�N�X���擾
    /// </summary>
    /// <returns></returns>
    Vector2Int GetPlayerIndex()
    {
        //�v�f����map.Length�Ŏ擾
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player") { return new Vector2Int(x, y); }
            }
        }
        // ������Ȃ��������̂��߂�-1��Ԃ�
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// num�z��̐������ړ�������
    /// </summary>
    /// <param name="tag">���������̃^�O</param>
    /// <param name="moveFrom">���������̃C���f�b�N�X</param>
    /// <param name="moveTo">��������̃C���f�b�N�X</param>
    /// <returns></returns>
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        // �ړ��悪�͈͊O�Ȃ�ړ��s��
        // �����Ȃ��������ɏ����A���^�[������B�������^�[��
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        // �ړ����4(��)��������
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            return false;
        }

        // �ړ����2(��)��������
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            // �ǂ̕����Ɉړ����邩���Z�o
            Vector2Int velocity = moveTo - moveFrom;
            // �v���C���[�̈ړ��悩��A����ɐ��2(��)���ړ�������B
            // ���̈ړ������BMoveNumber���\�b�h����MoveMenber���\�b�h��
            // �ĂсA�������ċN���Ă���B�ړ��s��bool�ŋL�^
            bool success = MoveNumber("Box", moveTo, moveTo + velocity);
            // ���������ړ����s������A�v���C���[�̈ړ������s
            if (!success) { return false; }
        }

        // �v���C���[�E���ւ�炸�̈ړ�����
        // field[moveFrom.y, moveFrom.x].transform.position = IndexToPosition(moveTo);

        Vector3 moveToPosition = IndexToPosition(moveTo);
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        // �p�[�e�B�N���Ăяo��
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
    /// �N���A������s��
    /// </summary>
    /// <returns></returns>
    bool IsCleard()
    {
        // Vector2Int�^�̉ϒ��z��
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // �i�[�ꏊ���ۂ��𔻒f
                if (map[y, x] == 3)
                {
                    // �i�[�ꏊ�̃C���f�b�N�X���T���Ă���
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        // �v�f����goals.Count�Ŏ擾
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // ��ł���������������������B��
                return false;
            }
        }

        // �������B���łȂ���Ώ����B��
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        // �z��̎��Ԃ̍쐬�Ə�����
        map = new int[,] { // 8*7�T�C�Y
            { 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 0, 0, 0, 0, 0, 0, 4 },
            { 4, 0, 0, 0, 2, 3, 0, 4 },
            { 4, 0, 1, 0, 2, 3, 0, 4 },
            { 4, 0, 0, 0, 2, 3, 0, 4 },
            { 4, 0, 0, 0, 0, 0, 0, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4 },
        };

        field = new GameObject[map.GetLength(0), map.GetLength(1)];

        // �ύX�B��dfor���œ񎟌��z��̏����o��
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
            // ���\�b�h�������������g�p
            Vector2Int playerIndex = GetPlayerIndex();

            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));

            // �Q�[���I�u�W�F�N�g��SetActive���\�b�h���g���L����
            clearText.SetActive(IsCleard());
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // ���\�b�h�������������g�p
            Vector2Int playerIndex = GetPlayerIndex();

            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ���\�b�h�������������g�p
            Vector2Int playerIndex = GetPlayerIndex();

            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // ���\�b�h�������������g�p
            Vector2Int playerIndex = GetPlayerIndex();

            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
        }

        // ���Z�b�g����
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

        // �Q�[���I�u�W�F�N�g��SetActive���\�b�h���g���L����
        clearText.SetActive(IsCleard());
    }
}
