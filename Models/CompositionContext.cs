using VirtualKeyboard.Input.Interfaces;

namespace VirtualKeyboard.Input.Models
{
    /// <summary>
    /// 조합 컨텍스트 (조합 상태 정보)
    /// </summary>
    public class CompositionContext
    {
        /// <summary>
        /// 현재 조합 상태
        /// </summary>
        public ICompositionState State { get; set; }

        /// <summary>
        /// 새 컨텍스트 생성
        /// </summary>
        public CompositionContext(ICompositionState state)
        {
            State = state ?? throw new System.ArgumentNullException(nameof(state));
        }

        /// <summary>
        /// 컨텍스트의 복사본 생성
        /// </summary>
        public CompositionContext Clone()
        {
            return new CompositionContext(State.Clone());
        }
    }
}
