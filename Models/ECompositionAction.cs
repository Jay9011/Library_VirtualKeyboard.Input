namespace VirtualKeyboard.Input.Models
{
    /// <summary>
    /// 조합 작업 유형
    /// </summary>
    public enum ECompositionAction
    {
        /// <summary>작업 없음</summary>
        None,
        /// <summary>문자 입력</summary>
        Input,
        /// <summary>문자 삭제</summary>
        Delete,
        /// <summary>조합 완료</summary>
        Commit,
        /// <summary>조합 취소</summary>
        Cancel,
        /// <summary>조합 업데이트</summary>
        Update
    }
}
