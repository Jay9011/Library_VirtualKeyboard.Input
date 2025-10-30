using VirtualKeyboard.Input.Interfaces;

namespace VirtualKeyboard.Input.Abstracts
{
    /// <summary>
    /// 조합 상태 기본 구현
    /// </summary>
    public abstract class CompositionStateBase : ICompositionState
    {
        /// <summary>
        /// 조합 중인지 여부
        /// </summary>
        public abstract bool IsComposing { get; }
        /// <summary>
        /// 상태 초기화
        /// </summary>
        public abstract void Reset();
        /// <summary>
        /// 상태 복사
        /// </summary>
        /// <returns>복사된 상태</returns>
        public abstract ICompositionState Clone();

        /// <summary>
        /// 상태를 문자열로 표현(디버깅용)
        /// </summary>
        public abstract override string ToString();
    }
}
