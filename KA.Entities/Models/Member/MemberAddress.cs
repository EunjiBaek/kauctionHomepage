using Newtonsoft.Json;
using System;

namespace KA.Entities.Models.Member
{
    /// <summary>
    /// 2022.02.27 / 박기준 - 도록 신청 프로세스 변경으로 ApplyBookYN, ApplyBookReceiver, ApplyBookContact 변수 추가
    /// </summary>
    public class MemberAddress
    {
        [JsonIgnore]
        public bool IsKor { get; set; }

        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonIgnore]
        public int MemUid { get; set; }

        [JsonProperty("primary")]
        public string Primary { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("etc")]
        public string Etc { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_name_kr")]
        public string CountryName { get; set; }

        [JsonProperty("country_name_en")]
        public string CountryName2 { get; set; }

        [JsonProperty("country_name")]
        public string DisplayCountryName => !IsKor && !string.IsNullOrEmpty(CountryName2) ? CountryName2 : CountryName;

        [JsonProperty("type_name_kr")]
        public string TypeName { get; set; }

        [JsonProperty("type_name_en")]
        public string TypeName2 { get; set; }

        [JsonProperty("type_name")]
        public string DisplayTypeName => !IsKor && !string.IsNullOrEmpty(TypeName2) ? TypeName2 : TypeName;

        [JsonProperty("zipcode")]
        public string ZipCode { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("receiver")]
        public string Receiver { get; set; }

        [JsonProperty("contact")]
        public string Contact { get; set; }

        /// <summary>
        /// 도록 배송지 주소 여부
        /// - 도록 신청 시 주소 목록에서 선택한 값을 Y로 지정 처리
        /// </summary>
        [JsonProperty("apply_book_yn")]
        public string ApplyBookYN { get; set; }

        /// <summary>
        /// 도록 구독자 이름
        /// </summary>
        [JsonProperty("apply_book_receiver")]
        public string ApplyBookReceiver { get; set; }

        /// <summary>
        /// 도록 구독자 연락처
        /// </summary>
        [JsonProperty("apply_book_contact")]
        public string ApplyBookContact { get; set; }

        [JsonIgnore]
        public int RegUid { get; set; }

        [JsonIgnore]
        public DateTime RegDate { get; set; }

        [JsonIgnore]
        public DateTime ModDate { get; set; }
    }
}
