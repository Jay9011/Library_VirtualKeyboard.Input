using VirtualKeyboard.Input.Models;

namespace VirtualKeyboard.Input.Interfaces
{
    /// <summary>
    /// 입력 조합기의 기본 인터페이스
    /// </summary>
    public interface IInputComposer
    {
        /// <summary>
        /// 조합기의 고유 이름
        /// 예: "QWERTY Korean", "CheonJiIn", "Romaji"
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 지원하는 언어 코드
        /// 예: "ko-KR", "ja-JP", "en-US"
        /// </summary>
        string Language { get; }

        /// <summary>
        /// 조합기의 설명
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 문자 입력 처리
        /// </summary>
        /// <param name="context">현재 조합 컨텍스트</param>
        /// <param name="input">입력된 문자열 (자모, 로마자 등)</param>
        /// <returns>조합 결과</returns>
        CompositionResult ProcessInput(CompositionContext context, string input);

        /// <summary>
        /// 백스페이스 입력 처리
        /// </summary>
        /// <param name="context">현재 조합 컨텍스트</param>
        /// <returns>조합 결과</returns>
        CompositionResult ProcessBackspace(CompositionContext context);

        /// <summary>
        /// 조합을 완료하고 확정
        /// </summary>
        /// <param name="context">현재 조합 컨텍스트</param>
        /// <returns>조합 결과</returns>
        CompositionResult Commit(CompositionContext context);

        /// <summary>
        /// 현재 조합 중인 내용을 취소
        /// </summary>
        /// <param name="context">현재 조합 컨텍스트</param>
        /// <returns>조합 결과</returns>
        CompositionResult Cancel(CompositionContext context);

        /// <summary>
        /// 새로운 조합 상태 객체 생성
        /// </summary>
        /// <returns>조합 상태 객체</returns>
        ICompositionState CreateState();

        /// <summary>
        /// 조합기 초기 상태로 리셋
        /// </summary>
        void Reset();

        /// <summary>
        /// 조합기가 특정 입력을 처리할 수 있는지 확인
        /// </summary>
        /// <param name="input">입력 문자열</param>
        /// <returns>처리 가능 여부</returns>
        bool CanProcess(string input);

        /// <summary>
        /// 특수 키 입력 처리
        /// </summary>
        /// <param name="context">현재 조합 컨텍스트</param>
        /// <param name="key">특수 키 문자 (예: '\b', '\n', '\r', ' ', '\t' 등)</param>
        /// <returns>처리 여부와 조합 결과. handled가 true면 InputMethod는 추가 처리를 하지 않습니다.</returns>
        (bool handled, CompositionResult result) TryProcessSpecialKey(CompositionContext context, char key);

        /// <summary>
        /// 변환 후보 선택 (일본어/중국어 IME 등)
        /// </summary>
        /// <param name="context">현재 조합 컨텍스트</param>
        /// <param name="candidateIndex">선택할 후보 인덱스</param>
        /// <returns>선택된 후보를 반영한 조합 결과</returns>
        CompositionResult SelectCandidate(CompositionContext context, int candidateIndex);
    }
}
