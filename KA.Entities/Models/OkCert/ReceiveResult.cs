namespace KA.Entities.Models.OkCert
{
    /// <summary>
    /// [#667/계획] ISMS 국내 회원가입 본인인증 선택으로 변경+응찰필수조건처리 - REDIRECT_URL 변수 추가
    /// [#797/계획] ISMS 대응 후 정책보완 후속 조치 - 본인인증 중복체크 관련 - INFO_CHANGE_NAME 변수 추가
    /// </summary>
    public class ReceiveResult
    {
        public bool RSLT { get; set; }

        public string REQUEST_KEY { get; set; }

        public string TX_SEQ_NO { get; set; }
        public string RSLT_CD { get; set; }
        public string RSLT_MSG { get; set; }

        public string CRD_CD { get; set; }
        public string CI_RQST_DT_TM { get; set; }

        public string RSLT_NAME { get; set; }
        public string RSLT_BIRTHDAY { get; set; }
        public string RSLT_SEX_CD { get; set; }
        public string RSLT_NTV_FRNR_CD { get; set; }

        public string DI { get; set; }
        public string CI { get; set; }
        public string CI2 { get; set; }
        public string CI_UPDATE { get; set; }
        public string TEL_COM_CD { get; set; }
        public string TEL_NO { get; set; }

        public string RETURN_MSG { get; set; }

        public string IS_MOBILE { get; set; }

        public string REQ_TYPE { get; set; }
        public int MEM_UID { get; set; }
        public string MEM_ID { get; set; }

        /// <summary>
        /// 인증 후 이동할 페이지 정보 변수
        /// </summary>
        public string REDIRECT_URL { get; set; }

        /// <summary>
        /// 인증 후 정보 변경 유무 처리 (메세지 처리)
        /// </summary>

        public string INFO_CHANGE_NAME { get; set; }
    }
}
