using VirtualKeyboard.Input.Models;

namespace VirtualKeyboard.Input.Extensions
{
    /// <summary>
    /// CompositionResult 확장 메서드
    /// </summary>
    public static class CompositionResultExtensions
    {
        /// <summary>
        /// 결과가 텍스트 변경을 포함하는지 확인
        /// </summary>
        public static bool HasTextChange(this CompositionResult result)
        {
            return result.Success &&
                   result.Action != ECompositionAction.None;
        }

        /// <summary>
        /// 버퍼가 있는지 확인
        /// </summary>
        public static bool HasBuffer(this CompositionResult result)
        {
            return !string.IsNullOrEmpty(result.Buffer);
        }

        /// <summary>
        /// 조합 중인지 확인
        /// </summary>
        public static bool IsComposing(this CompositionResult result)
        {
            return result.Success &&
                   result.HasBuffer();
        }
    }
}
