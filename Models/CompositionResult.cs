using System;
using System.Collections.Generic;

namespace VirtualKeyboard.Input.Models
{
    /// <summary>
    /// 조합 결과를 나타내는 불변 구조체
    /// </summary>
    public readonly struct CompositionResult : IEquatable<CompositionResult>
    {
        /// <summary>
        /// 조합 성공 여부
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 확정된 텍스트 (텍스트 버퍼에 삽입할 문자)
        /// 이번 입력에서 확정된 문자만 포함 (이전 확정 내용 없음)
        /// </summary>
        public string CommittedText { get; }

        /// <summary>
        /// 조합 중인 텍스트 (밑줄 표시, 아직 확정 안됨)
        /// 예: 한글 "가", 일본어 "か"
        /// </summary>
        public string ComposingText { get; }

        /// <summary>
        /// 조합 버퍼 (아직 확정되지 않은 입력, 주로 밑줄 표시)
        /// 예: 일본어 로마자 입력 시 "k" → 버퍼에 저장
        /// </summary>
        public string Buffer { get; }

        /// <summary>
        /// 오류 메시지 (실패 시)
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 작업 유형
        /// </summary>
        public ECompositionAction Action { get; }

        /// <summary>
        /// 변환 후보 목록 (일본어/중국어 IME 등)
        /// </summary>
        public IReadOnlyList<string> Candidates { get; }

        /// <summary>
        /// 선택된 후보 인덱스 (-1이면 선택 안됨)
        /// </summary>
        public int SelectedCandidateIndex { get; }

        /// <summary>
        /// CompositionResult 생성
        /// </summary>
        private CompositionResult(
            bool success,
            string committedText,
            string composingText,
            string buffer,
            string errorMessage,
            ECompositionAction action,
            IReadOnlyList<string> candidates,
            int selectedCandidateIndex)
        {
            Success = success;
            CommittedText = committedText ?? string.Empty;
            ComposingText = composingText ?? string.Empty;
            Buffer = buffer ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            Action = action;
            Candidates = candidates ?? Array.Empty<string>();
            SelectedCandidateIndex = selectedCandidateIndex;
        }

        /// <summary>
        /// 성공 결과 생성
        /// </summary>
        /// <param name="composingText">조합 중인 텍스트</param>
        /// <param name="committedText">확정된 텍스트 (이번 입력에서 확정된 것만)</param>
        /// <param name="buffer">조합 버퍼</param>
        /// <param name="action">작업 유형</param>
        /// <param name="candidates">변환 후보 목록</param>
        /// <param name="selectedCandidateIndex">선택된 후보 인덱스</param>
        public static CompositionResult Succeeded(
            string composingText,
            string committedText = "",
            string buffer = "",
            ECompositionAction action = ECompositionAction.Input,
            IReadOnlyList<string> candidates = null,
            int selectedCandidateIndex = -1)
        {
            return new CompositionResult(
                success: true,
                committedText: committedText,
                composingText: composingText,
                buffer: buffer,
                errorMessage: string.Empty,
                action: action,
                candidates: candidates,
                selectedCandidateIndex: selectedCandidateIndex
            );
        }

        /// <summary>
        /// 실패 결과 생성
        /// </summary>
        public static CompositionResult Failed(string errorMessage = "")
        {
            return new CompositionResult(
                success: false,
                committedText: string.Empty,
                composingText: string.Empty,
                buffer: string.Empty,
                errorMessage: errorMessage,
                action: ECompositionAction.None,
                candidates: null,
                selectedCandidateIndex: -1
            );
        }

        /// <summary>
        /// 변경 없음 결과 생성
        /// </summary>
        public static CompositionResult NoChange()
        {
            return new CompositionResult(
                success: true,
                committedText: string.Empty,
                composingText: string.Empty,
                buffer: string.Empty,
                errorMessage: string.Empty,
                action: ECompositionAction.None,
                candidates: null,
                selectedCandidateIndex: -1
            );
        }

        /// <summary>
        /// 동일성 비교
        /// </summary>
        /// <param name="other">비교할 결과</param>
        /// <returns>동일성 여부</returns>
        public bool Equals(CompositionResult other)
        {
            return Success == other.Success &&
                   CommittedText == other.CommittedText &&
                   ComposingText == other.ComposingText &&
                   Buffer == other.Buffer &&
                   Action == other.Action &&
                   SelectedCandidateIndex == other.SelectedCandidateIndex &&
                   CandidatesEqual(Candidates, other.Candidates);
        }

        /// <summary>
        /// 후보 목록 동일성 비교
        /// </summary>
        /// <param name="a">비교할 후보 목록</param>
        /// <param name="b">비교할 후보 목록</param>
        /// <returns>동일성 여부</returns>
        private static bool CandidatesEqual(IReadOnlyList<string> a, IReadOnlyList<string> b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// 동일성 비교
        /// </summary>
        /// <param name="obj">비교할 객체</param>
        /// <returns>동일성 여부</returns>
        public override bool Equals(object obj)
        {
            return obj is CompositionResult other && Equals(other);
        }

        /// <summary>
        /// 해시 코드 생성
        /// </summary>
        /// <returns>해시 코드</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Success.GetHashCode();
                hash = hash * 31 + (CommittedText?.GetHashCode() ?? 0);
                hash = hash * 31 + (ComposingText?.GetHashCode() ?? 0);
                hash = hash * 31 + (Buffer?.GetHashCode() ?? 0);
                hash = hash * 31 + Action.GetHashCode();
                hash = hash * 31 + SelectedCandidateIndex.GetHashCode();

                if (Candidates != null)
                {
                    foreach (var candidate in Candidates)
                    {
                        hash = hash * 31 + (candidate?.GetHashCode() ?? 0);
                    }
                }

                return hash;
            }
        }

        public static bool operator ==(CompositionResult left, CompositionResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CompositionResult left, CompositionResult right)
        {
            return !left.Equals(right);
        }
    }
}
