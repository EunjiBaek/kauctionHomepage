using KA.Entities.Attributes;
using KA.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KA.Entities.Models.Main
{
    public class Banner
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("image_file_name")]
        public string ImageFileName { get; set; }

        [JsonProperty("image_file_name_en")]
        public string ImageFileNameEn { get; set; }

        [JsonProperty("image_file_path")]
        public string ImageFilePath { get; set; }

        [JsonProperty("mobile_image_file_name")]
        public string MobileImageFileName { get; set; }

        [JsonProperty("mobile_image_file_name_en")]
        public string MobileImageFileNameEn { get; set; }

        [JsonProperty("mobile_image_file_path")]
        public string MobileImageFilePath { get; set; }

        [JsonProperty("display_image_file_name")]        
        public string DisplayImageFileName
        {
            get
            {
                if (IsMobile)
                {
                    // 모바일이며 모바일영문 이미지 체크
                    if (!IsKor && !string.IsNullOrWhiteSpace(MobileImageFileNameEn))
                    {
                        return MobileImageFileNameEn;
                    }
                    else
                    {
                        return !string.IsNullOrWhiteSpace(MobileImageFileName) ? MobileImageFileName : ImageFileName;
                    }
                }
                else
                {
                    // 피시이며 영문 이미지 체크
                    return !IsKor && !string.IsNullOrWhiteSpace(ImageFileNameEn) ? ImageFileNameEn : ImageFileName;
                }
            }
        }

        [JsonProperty("image_bg_color")]
        public string ImageBgColor { get; set; }

        [JsonProperty("layer_title_kr")]
        public string LayerTitle { get; set; }

        [JsonProperty("layer_title_en")]
        public string LayerTitleEn { get; set; }

        [JsonProperty("layer_title")]
        public string DisplayLayerTitle => !IsKor && !string.IsNullOrWhiteSpace(LayerTitleEn) ? LayerTitleEn : LayerTitle;

        [JsonProperty("layer_sub_title_kr")]
        public string LayerSubTitle { get; set; }

        [JsonProperty("layer_sub_title_en")]
        public string LayerSubTitleEn { get; set; }

        [JsonProperty("layer_sub_title")]
        public string DisplayLayerSubTitle => !IsKor && !string.IsNullOrWhiteSpace(LayerSubTitleEn) ? LayerSubTitleEn : LayerSubTitle;

        [JsonIgnore]
        public string LayerContent { get; set; }

        [JsonIgnore]
        public string LayerContentEn { get; set; }

        [JsonIgnore]
        public string DisplayLayerContent => !IsKor && !string.IsNullOrWhiteSpace(LayerContentEn) ? LayerContentEn : LayerContent;

        [JsonIgnore]
        public bool IsDisplayLayer => IsKor ? UseLayer.Equals("A") || UseLayer.Equals("K") : UseLayer.Equals("A") || UseLayer.Equals("E");

        [JsonProperty("start_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 게시 종료 조건 플래그
        /// </summary>
        [JsonProperty("end_flag")]
        public string EndFlag { get; set; }

        /// <summary>
        /// 게시 종료 일자
        /// </summary>
        [JsonProperty("end_date")]
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd HH:mm")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 게시 종료 이벤트 선택 경매
        /// </summary>
        [JsonProperty("end_auc_uid")]
        public int EndAucUid { get; set; }

        /// <summary>
        /// 게시 종료 이벤트 발동 조건
        /// </summary>
        [JsonProperty("end_auc_opt")]
        public string EndAucOpt { get; set; }

        /// <summary>
        /// 게시 종료 이벤트 발동 조건 값
        /// </summary>
        [JsonProperty("end_auc_val")]
        public string EndAucVal { get; set; }

        [JsonProperty("use_layer")]
        public string UseLayer { get; set; }

        [JsonProperty("use_flag")]
        public string UseFlag { get; set; }

        [JsonProperty("use_link_work")]
        public string UseLinkWork { get; set; }

        [JsonProperty("link_work_uid")]
        public int LinkWorkUid { get; set; }

        [JsonProperty("link_work_auc_kind")]
        public string LinkWorkAucKind { get; set; }

        [JsonProperty("link_work_auc_num")]
        public int LinkWorkAucNum { get; set; }

        [JsonProperty("link_work_lot_num")]
        public int LinkWorkLotNum { get; set; }

        [JsonProperty("link_work_title_kr")]
        public string LinkWorkTitle { get; set; }

        [JsonProperty("link_work_title_en")]
        public string LinkWorkTitleEn { get; set; }

        [JsonProperty("link_work_title")]
        public string DisplayLinkWorkTitle => !IsKor && !string.IsNullOrWhiteSpace(LinkWorkTitleEn) ? LinkWorkTitleEn : LinkWorkTitle;

        [JsonProperty("link_work_artist_name_kr")]
        public string LinkWorkArtistName { get; set; }

        [JsonProperty("link_work_artist_name_en")]
        public string LinkWorkArtistNameEn { get; set; }

        [JsonProperty("link_work_artist_name")]
        public string DisplayLinkWorkArtistName => !IsKor && !string.IsNullOrWhiteSpace(LinkWorkArtistNameEn) ? LinkWorkArtistNameEn : LinkWorkArtistName;

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("reg_uid")]
        public int RegUid { get; set; }

        [JsonProperty("reg_date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime RegDate { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("is_kor")]
        public bool IsKor => !string.IsNullOrWhiteSpace(Lang) && Lang.Equals("K");

        [JsonProperty("is_mobile")]
        public bool IsMobile { get; set; }

        [JsonIgnore]
        public string ButtonInfo { get; set; }

        [JsonIgnore]
        public string ButtonInfoEn { get; set; }

        [JsonIgnore]
        public IEnumerable<BannerButton> DisplayButtonInfo
        {
            get
            {
                var value = IsKor ? ButtonInfo : ButtonInfoEn;
                List<BannerButton> bannerButtons = new List<BannerButton>();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    foreach (var item in value.Split('^'))
                    {
                        var useFlag = item.Split('|')[4];
                        if (useFlag.Equals("A") || useFlag.Equals(IsKor ? "K" : "E"))
                        {
                            bannerButtons.Add(new BannerButton
                            {
                                Order = int.Parse(item.Split('|')[0]),
                                Text = StringHelper.RemoveTag(item.Split('|')[1]),
                                LinkUrl = item.Split('|')[2],
                                TargetBlank = item.Split('|')[3],
                                UseFlag = item.Split('|')[4]
                            });
                        }
                    }
                }
                return bannerButtons;
            }
        }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
