namespace WOAT
{
    public class ToolsForPlaying
    {
        /*
        //�̺�Ʈ �븮��
        using System;

        public class Program
        {
            // �븮�� ����
            public delegate void SimpleDelegate();

            // �̺�Ʈ ����: SimpleDelegate ������ �޼��带 ����ų �� �ִ� �̺�Ʈ
            public static event SimpleDelegate OnGreet;

            // �޼���: �̺�Ʈ�� �߻��� �� ����� ����
            public static void SayHello()
            {
                Console.WriteLine("Hello, world!");
            }

            public static void Main(string[] args)
            {
                // �̺�Ʈ�� �޼��带 ���� (����)
                OnGreet += SayHello;

                // ������ �����Ǹ� �̺�Ʈ ȣ�� (���� ��ȭ�� ���� �ڵ� ȣ��)
                if (OnGreet != null)  // ������ �޼��尡 �ִ��� Ȯ��
                {
                    OnGreet();  // �̺�Ʈ �߻� (SayHello�� �ڵ����� ȣ���)
                }
            }
        }

        // �̺�Ʈ ������
        // ���̵���� ����
        public event Action<bool> OnRidingStatusChanged;

        protected virtual void ToggleRidingStatus()
        {
            isRiding = !isRiding;
            OnRidingStatusChanged?.Invoke(isRiding); // ���� ��ȭ�� �÷��̾�� ����
        }

        // Player Ŭ���� ����
        public void RegisterRidingGear(RidingGear ridingGear)
        {
            ridingGear.OnRidingStatusChanged += (status) => { isRiding = status; };
        }

        // �������̽�
        IMovable
            ����, ��
        IJumpable
            ����,
        IDrivable
            ��, ƿƮ

        // �̱��� ��Ʈ�ѷ�

        �÷��̾��� ���
        Move
            ���� �̵�
            WASD ��
        Run
            ���� �̵�
            WSŰ�� ��� ����, ��쿡 ���� modifier �ʿ�
        Jump
            ���� �̵�
            Space
        Eyes
            ȸ��
            ���콺 ����
        ��ȣ�ۿ�
            ��Ÿ Ű �Է�
        ��Ÿ ����
            ���̵����� ��ȣ�ۿ�

        ���̵������ ���
        RideOn
        RideOff
        ���� ����ȭ

        ���� Ÿ��
        Move
        Turn
        Tilt
        Enhance Mode AA
            ��ֹ� Ž��
            ��ֹ� ��ġ ����
            ȸ�� ��� ���
            ȸ�� �⵿
            Ʈ�� ����

        ���� Ÿ��
        ����
        ����
        ��
        ������
        ��ȣ�ۿ�
        ���ڽ� ���
            �� Ž��
            �ٷ� ����
            �߷� ��Ʈ��
        */
    }
}