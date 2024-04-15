using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // �z��̐錾
    int[] map;

    // �N���X�̒��́A���\�b�h�̊O�ɏ������Ƃɒ���

    /// <summary>
    /// �R���\�[���ɕ\��������
    /// </summary>
    void PrintArray()
    {
        // �ǉ��B������̐錾�Ə�����
        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            // �ύX�B������Ɍ������Ă���
            debugText += map[i].ToString() + ",";
        }
        // ����������������o��
        Debug.Log(debugText);
    }

    // �Ԃ�l�̌^�ɒ���

    /// <summary>
    /// �v���C���[�̃C���f�b�N�X���擾
    /// </summary>
    /// <returns></returns>
    int GetPlayerIndex()
    {
        //�v�f����map.Length�Ŏ擾
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        // ������Ȃ��������̂��߂�-1��Ԃ�
        return -1;
    }

    /// <summary>
    /// num�z��̐������ړ�������
    /// </summary>
    /// <param name="number">����������</param>
    /// <param name="moveFrom">���������̃C���f�b�N�X</param>
    /// <param name="moveTo">��������̃C���f�b�N�X</param>
    /// <returns></returns>
    bool MoveNumber(int number, int moveFrom, int moveTo)
    {
        if (moveTo < 0 || moveTo >= map.Length)
        {
            // �����Ȃ��������ɏ����A���^�[������B�������^�[��
            return false;
        }
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �z��̎��Ԃ̍쐬�Ə�����
        map = new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0 };

        PrintArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // ���\�b�h�������������g�p
            int playerIndex = GetPlayerIndex();

            //�ړ��������֐���
            MoveNumber(1, playerIndex, playerIndex + 1);

            PrintArray();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // ���\�b�h�������������g�p
            int playerIndex = GetPlayerIndex();

            //�ړ��������֐���
            MoveNumber(1, playerIndex, playerIndex - 1);

            PrintArray();
        }
    }
}
