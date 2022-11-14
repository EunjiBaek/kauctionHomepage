using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.07.21 [#718/개선] 회원 중 임직원 여부 설정 옵션값 관련 신규정의 반영 
    /// - EmployeeUid, EmployeeName, EmployeeTypCd 추가
    /// - 관리자 - 회원정보 상세 페이지에서 담당자를 설정할 수 있었으나, 케이오피스에서 관린하는 것으로 변경 (현재 케이오피스 설정값 조회)
    /// </summary>
    public class MemberKOffice
    {
        public int WwwSeq { get; set; }

        public int MemUid { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime RetireDate { get; set; }

        public string MemBizTypCd { get; set; }

        public string MemBizTypName { get; set; }

        public string MemName { get; set; }

        public string BirthDate { get; set; }

        public string BirthDateType { get; set; }

        public string Sex { get; set; }

        public string HTel { get; set; }

        public string RegTelNo { get; set; }

        public string Email { get; set; }

        public string Countr { get; set; }

        public string MemSignupTypCd { get; set; }

        public string MemSignupTypName { get; set; }

        public string MemAuthMthdCd { get; set; }

        public string MemAuthMthdName { get; set; }

        public string MngApprover { get; set; }

        /// <summary>
        /// 담당자 고유 번호
        /// </summary>
        public int EmployeeUid { get; set; }

        /// <summary>
        /// 담당자 이름
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 담당자 유형 (001: 본인, 002: 관계자)
        /// </summary>
        public string EmployeeTypCd { get; set; }
    }
}
