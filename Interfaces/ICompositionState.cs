namespace VirtualKeyboard.Input.Interfaces
{
    /// <summary>
    /// 조합 상태 인터페이스
    /// </summary>
    public interface ICompositionState
    {
        /// <summary>현재 조합 중인지 여부</summary>
        bool IsComposing { get; }
        /// <summary>상태 초기화</summary>
        void Reset();
        /// <summary>
        /// 상태의 복사본을 생성
        /// </summary>
        /// <returns>복사된 상태 객체</returns>
        ICompositionState Clone();
    }
}
