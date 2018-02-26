using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using HRM.Business.Category.Domain;
using HRM.Business.Category.Models;
using HRM.Infrastructure.Utilities;
using HRM.Presentation.Category.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VnResource.Helper.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Reflection;
using System.Collections;
using HRM.Business.Main.Domain;
using HRM.Presentation.Service;
using System.Web.Caching;
using System.Web;
using HRM.Presentation.Payroll.Models;
using HRM.Business.Payroll.Models;
using System.Data.SqlTypes;
using VnResource.Helper.Linq;
using HRM.Infrastructure.Utilities.Helper;
using HRM.Presentation.Hr.Models;
using HRM.Business.Evaluation.Models;
using System.Web.Script.Serialization;
using VnResource.Helper.Utility;
using System.Xml;
using System.IO;
using HRM.Business.Payroll.Domain;
using HRM.Business.HrmSystem.Domain;
using HRM.Business.Finance.Domain;
using HRM.Business.Hr.Models;
using HRM.Business.Recruitment.Models;
using HRM.Presentation.Recruitment.Models;
using HRM.Business.Hr.Domain;
using HRM.Business.HrmSystem.Models;
using HRM.Presentation.Attendance.Models;
using VnResource.Helper.Setting;
using HRM.Presentation.Evaluation.Models;
using Newtonsoft.Json.Linq;
using Kendo.Mvc;
using HRM.Business.Training.Domain;
using HRM.Presentation.HrmSystem.Models;
using HRM.Business.Insurance.Models;
using HRM.Presentation.Insurance.Models;
using HRM.Infrastructure.Utilities;

namespace HRM.Presentation.Hr.Service.Controllers
{
    public class Cat_GetDataController : BaseController
    {
        string Hrm_Main_Web = System.Configuration.ConfigurationManager.AppSettings["Hrm_Main_Web"];
        BaseService baseService = new BaseService();
        string _status = string.Empty;




        [HttpPost]
        public ActionResult GetDatasurveyProfile(string surveyProfileID)
        {
            if (!string.IsNullOrEmpty(surveyProfileID))
            {
                var _SurveyProfileServices = new Sur_SurveyProfileServices();
                var surVey = _SurveyProfileServices.GetDataSurveyProfileByID(Guid.Parse(surveyProfileID));
                return Json(surVey, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        public JsonResult SaveQuestion(FormCollection formCollection)
        {
            string message = "Success";
            Cat_SurveyQuestionEntity objQuestion = new Cat_SurveyQuestionEntity();
            Cat_SurrveyServices surveyService = new Cat_SurrveyServices();
            message = surveyService.SaveSurveyProfile(formCollection);
            return Json(message);
        }

        public JsonResult SaveQuestionForStopWorking(FormCollection formCollection)
        {
            string message = "Success";
            Cat_SurveyQuestionEntity objQuestion = new Cat_SurveyQuestionEntity();
            Cat_SurrveyServices surveyService = new Cat_SurrveyServices();
            message = surveyService.SaveSurveyProfileForStopWorking(formCollection);
            return Json(message);
        }

        public ActionResult GetSurveyProfile(string SurveyProfileID)
        {
            Cat_SurrveyServices surveyService = new Cat_SurrveyServices();
            var entity = surveyService.GetSurveyProfileById(Guid.Parse(SurveyProfileID));
            var model = new Cat_SurveyModel();
            if (entity != null)
            {
                model = entity.CopyData<Cat_SurveyModel>();
                model.Questions = new List<Cat_SurveyQuestionModel>();
                foreach (var objQuestion in entity.Questions)
                {
                    var objQuestionEntity = objQuestion.CopyData<Cat_SurveyQuestionModel>();
                    objQuestionEntity.QuestionDetails = objQuestion.QuestionDetails.Translate<Cat_QuestionDetailModel>();
                    objQuestionEntity.QuestionProfileDetails = objQuestion.QuestionProfileDetails.Translate<Sur_SurveyProfileDetailModel>();
                    model.Questions.Add(objQuestionEntity);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSurveyQuestionBySurveyId(Guid? SurveyId)
        {
            if (SurveyId != null)
            {
                string status = string.Empty;
                var model = new Cat_SurveyModel();
                var service = new ActionService(UserLogin);
                var surveyService = new Cat_SurrveyServices();
                var entity = surveyService.GetSurveyById(SurveyId.Value, null);
                if (entity != null)
                {
                    model = entity.CopyData<Cat_SurveyModel>();
                    model.Questions = new List<Cat_SurveyQuestionModel>();
                    foreach (var objQuestion in entity.Questions)
                    {
                        var objQuestionEntity = objQuestion.CopyData<Cat_SurveyQuestionModel>();
                        objQuestionEntity.QuestionDetails = objQuestion.QuestionDetails.Translate<Cat_QuestionDetailModel>();
                        objQuestionEntity.QuestionProfileDetails = objQuestion.QuestionProfileDetails.Translate<Sur_SurveyProfileDetailModel>();
                        model.Questions.Add(objQuestionEntity);
                    }
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGradeSalDeptMulti(string text)
        {
            return GetDataForControl<Cat_GradeSalDeptMultiModel, Cat_GradeSalDeptMultiEntity>(text, ConstantSql.hrm_cat_sp_get_GradeSalDeptMulti);
        }

        public JsonResult GetFacilityIssueCategoryMulti(string text)
        {
            return GetDataForControl<Cat_FacilityIssuesCategoryMultiModel, Cat_FacilityIssuesCategoryMultiModel>(text, ConstantSql.hrm_cat_sp_get_FacilityIssuesCategory_Multi);
        }

        public JsonResult JoinTimeInDate(DateTime date, string time)
        {
            if (time == null)
                return Json(date, JsonRequestBehavior.AllowGet);

            var _arr = time.Split(':');
            if (_arr[0].ToString().Equals("__"))
                _arr[0] = "00";
            if (_arr[1].ToString().Equals("__"))
                _arr[1] = "00";
            if (_arr[2].ToString().Equals("__"))
                _arr[2] = "00";
            TimeSpan _time = new TimeSpan(int.Parse(_arr[0].ToString()), int.Parse(_arr[1].ToString()), int.Parse(_arr[2].ToString()));
            double _hours = _time.TotalHours;

            return Json(date.AddHours(_hours), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTemplateImport(Guid? ExportID)
        {
            List<object> lstResult = new List<object>();
            lstResult.AddRange(new object[3]);
            string status = "";
            ActionService service = new ActionService(UserLogin, LanguageCode);
            var exportEntity = service.GetData<Cat_ImportEntity>(ExportID, ConstantSql.hrm_cat_sp_get_ImportById, ref status).FirstOrDefault();
            if (!string.IsNullOrEmpty(exportEntity.TemplateFile))
                lstResult[0] = "<a href='" + ConstantPathWeb.Hrm_Main_Web + "/Templates/" + exportEntity.TemplateFile + "'>DownLoadTemplate</a>";
            if (!string.IsNullOrEmpty(exportEntity.ProcessDuplicateData))
                lstResult[1] = exportEntity.ProcessDuplicateData;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(exportEntity.ID);
            var lstImportItemEntity = baseService.GetData<Cat_ImportItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ImportItemByImportID, UserLogin, ref status);
            lstImportItemEntity = lstImportItemEntity.Where(d => d.DuplicateGroup.HasValue).ToList();
            var lstGroupDuplicate = lstImportItemEntity.GroupBy(d => d.DuplicateGroup.Value).ToList();
            string strResult = "";
            strResult += "<table>";
            foreach (var item in lstGroupDuplicate)
            {
                strResult += "<tr>";
                strResult += "<td>" + item.Key + ": ";
                strResult += "</td>";
                string strFields = "";
                foreach (var objImportItem in item)
                {
                    strFields += objImportItem.ChildFieldLevel1 + ",";
                }
                strResult += "<td>" + strFields;
                strResult += "</td>";
                strResult += "<tr>";
            }
            lstResult[2] = strResult;
            return Json(lstResult);
        }
        #region GroupRoster
        public ActionResult GetAttRosterGroup()
        {
            ActionService _ser = new ActionService(UserLogin);
            Sys_AttOvertimePermitConfigServices sysServices = new Sys_AttOvertimePermitConfigServices();
            //string listcode = sysServices.GetConfigValue<string>(AppConfig.HRM_ATT_CONFIG_SHIFT_CODE_ROSTERGROUP);
            //  [21/11/2015][Phuc.Nguyen][Bug][0060454] Load combobox khi đã cấu hình chấm công
            string listcode = sysServices.GetConfigValue<string>(AppConfig.HRM_ATT_CONFIG_NAME_ROSTERGROUP);
            if (string.IsNullOrEmpty(listcode))
            {
                return Json(null);
            }
            var array = listcode.Split(',').ToList();
            return Json(array);
        }
        #endregion
        #region Cat_SeniorityRank
        [HttpPost]
        public ActionResult GetSeniorityRankList([DataSourceRequest] DataSourceRequest request, Cat_SeniorityRankSearchModel model)
        {
            return GetListDataAndReturn<Cat_SeniorityRankModel, Cat_SeniorityRankEntity, Cat_SeniorityRankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SeniorityRank);
        }
        public ActionResult ExportSeniorityRankSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SeniorityRankEntity, Cat_SeniorityRankModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SeniorityRankByIds);
        }
        public ActionResult ExportAllSeniorityRankList([DataSourceRequest] DataSourceRequest request, Cat_SeniorityRankSearchModel model)
        {
            return ExportAllAndReturn<Cat_SeniorityRankEntity, Cat_SeniorityRankModel, Cat_SeniorityRankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SeniorityRank);
        }
        #endregion
        #region Cat_SourceAds
        [HttpPost]
        public JsonResult GetMultiSourceAds(string text)
        {
            return GetDataForControl<Cat_SourceAdsMultiModel, Cat_SourceAdsMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SourceAds_Multi);
        }

        public ActionResult GetSourceAdsList([DataSourceRequest] DataSourceRequest request, Cat_SourceAdsSearchModel model)
        {
            return GetListDataAndReturn<Cat_SourceAdsModel, Cat_SourceAdsEntity, Cat_SourceAdsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SourceAds);
        }
        public ActionResult ExportSourceAdsSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SourceAdsEntity, Cat_SourceAdsModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SourceAdsByIds);
        }
        public ActionResult ExportAllSourceAdsList([DataSourceRequest] DataSourceRequest request, Cat_SourceAdsSearchModel model)
        {
            return ExportAllAndReturn<Cat_SourceAdsEntity, Cat_SourceAdsModel, Cat_SourceAdsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SourceAds);
        }
        #endregion
        #region Cat_Fabric
        [HttpPost]
        public JsonResult GetMultiFabric(string text)
        {
            return GetDataForControl<Cat_FabricMultiModel, Cat_FabricMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Fabric_Multi);
        }

        public ActionResult GetFabricList([DataSourceRequest] DataSourceRequest request, Cat_FabricSearchModel model)
        {
            return GetListDataAndReturn<Cat_FabricModel, Cat_FabricEntity, Cat_FabricSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Fabric);
        }
        public ActionResult ExportFabricSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_FabricEntity, Cat_FabricModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_FabricByIds);
        }
        public ActionResult ExportAllFabricList([DataSourceRequest] DataSourceRequest request, Cat_FabricSearchModel model)
        {
            return ExportAllAndReturn<Cat_FabricEntity, Cat_FabricModel, Cat_FabricSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Fabric);
        }
        #endregion
        #region Cat_Wash
        [HttpPost]
        public JsonResult GetMultiWash(string text)
        {
            return GetDataForControl<Cat_WashMultiModel, Cat_WashMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Wash_Multi);
        }

        public ActionResult GetWashList([DataSourceRequest] DataSourceRequest request, Cat_WashSearchModel model)
        {
            return GetListDataAndReturn<Cat_WashModel, Cat_WashEntity, Cat_WashSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Wash);
        }
        public ActionResult ExportWashSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_WashEntity, Cat_WashModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_WashByIds);
        }
        public ActionResult ExportAllWashList([DataSourceRequest] DataSourceRequest request, Cat_WashSearchModel model)
        {
            return ExportAllAndReturn<Cat_WashEntity, Cat_WashModel, Cat_WashSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Wash);
        }
        #endregion
        #region Cat_ProductPart
        public ActionResult GetProductPartList([DataSourceRequest] DataSourceRequest request, Cat_ProductPartSearchModel model)
        {
            return GetListDataAndReturn<Cat_ProductPartModel, Cat_ProductPartEntity, Cat_ProductPartSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductPart);
        }
        public ActionResult ExportProductPartSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProductPartEntity, Cat_ProductPartModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ProductPartByIds);
        }
        public ActionResult ExportAllProductPartList([DataSourceRequest] DataSourceRequest request, Cat_ProductPartSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProductPartEntity, Cat_ProductPartModel, Cat_ProductPartSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductPart);
        }
        public JsonResult GetMultiProductPart(string text)
        {
            return GetDataForControl<Cat_ProductPartModel, Cat_ProductPartEntity>(text, ConstantSql.hrm_cat_sp_get_ProductPart_Multi);
        }
        #endregion
        #region Cat_PartProductItem
        public ActionResult GetPartProductItemByProductItemIDList([DataSourceRequest] DataSourceRequest request, Guid ProductItemID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ProductItemID);
            var result = baseService.GetData<Cat_PartProductItemEntity>(objs, ConstantSql.hrm_cat_sp_get_PartProductItemByProductItemID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));

        }
        public ActionResult GetFistProductPartPriceByProductPartID(Guid ProductPartID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ProductPartID);
            var objresult = new Cat_ProductPartPriceEntity();
            var lstresult = baseService.GetData<Cat_ProductPartPriceEntity>(objs, ConstantSql.hrm_cat_sp_get_Cat_ProductPartPriceByProductPartID, UserLogin, ref status);
            if (lstresult != null && lstresult.Count > 0)
            {
                //lấy với ngày hiệu lực gần nhất
                objresult = lstresult.OrderByDescending(s => s.DateEffect).FirstOrDefault();
            }
            return Json(objresult);
        }
        #endregion
        #region Cat_ProductPartPrice
        public ActionResult GetProductPartPriceByProductPartIDList([DataSourceRequest] DataSourceRequest request, Guid ProductPartID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ProductPartID);
            var result = baseService.GetData<Cat_ProductPartPriceEntity>(objs, ConstantSql.hrm_cat_sp_get_Cat_ProductPartPriceByProductPartID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));

        }
        #endregion
        #region Cat_SubjectGroup
        public ActionResult GetSubjectGroupList([DataSourceRequest] DataSourceRequest request, Cat_SubjectGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_SubjectGroupModel, Cat_SubjectGroupEntity, Cat_SubjectGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SubjectGroup);
        }
        public ActionResult ExportSubjectGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SubjectGroupEntity, Cat_SubjectGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SubjectGroupByIds);
        }
        public ActionResult ExportAllSubjectGroupList([DataSourceRequest] DataSourceRequest request, Cat_SubjectGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_SubjectGroupEntity, Cat_SubjectGroupModel, Cat_SubjectGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SubjectGroup);
        }


        #endregion
        #region Cat_Subject
        public ActionResult GetSubjectList([DataSourceRequest] DataSourceRequest request, Cat_SubjectSearchModel model)
        {
            return GetListDataAndReturn<Cat_SubjectModel, Cat_SubjectEntity, Cat_SubjectSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Subject);
        }
        public ActionResult ExportSubjectSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SubjectEntity, Cat_SubjectModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SubjectByIds);
        }
        public ActionResult ExportAllSubjectList([DataSourceRequest] DataSourceRequest request, Cat_SubjectSearchModel model)
        {
            return ExportAllAndReturn<Cat_SubjectEntity, Cat_SubjectModel, Cat_SubjectSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Subject);
        }


        #endregion
        #region Cat_ExchangeRate
        public ActionResult GetExchangeRateList([DataSourceRequest] DataSourceRequest request, Cat_ExchangeRateSearchModel model)
        {
            return GetListDataAndReturn<Cat_ExchangeRateModel, Cat_ExchangeRateEntity, Cat_ExchangeRateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ExchangeRate);
        }
        public ActionResult ExportExchangeRateSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ExchangeRateEntity, Cat_ExchangeRateModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ExchangeRateByIds);
        }
        public ActionResult ExportAllExchangeRateList([DataSourceRequest] DataSourceRequest request, Cat_ExchangeRateSearchModel model)
        {
            return ExportAllAndReturn<Cat_ExchangeRateEntity, Cat_ExchangeRateModel, Cat_ExchangeRateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ExchangeRate);
        }
        #endregion
        #region Cat_Village
        public ActionResult GetVillageList([DataSourceRequest] DataSourceRequest request, Cat_VillageSearchModel model)
        {
            return GetListDataAndReturn<Cat_VillageModel, Cat_VillageEntity, Cat_VillageSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Village);
        }
        public ActionResult ExportVillageSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_VillageEntity, Cat_VillageModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_VillageByIds);
        }
        public ActionResult ExportAllVillageList([DataSourceRequest] DataSourceRequest request, Cat_VillageSearchModel model)
        {
            return ExportAllAndReturn<Cat_VillageEntity, Cat_VillageModel, Cat_VillageSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Village);
        }
        #endregion
        #region Cat_ReceivePlace
        public ActionResult GetListReceivePlace([DataSourceRequest] DataSourceRequest request, Cat_ReceivePlaceSearchModel model)
        {
            return GetListDataAndReturn<Cat_ReceivePlaceModel, Cat_ReceivePlaceEntity, Cat_ReceivePlaceSearchModel>(request, model, ConstantSql.hrm_Cat_SP_GET_RECEIVEPLACE);
        }

        public ActionResult ExportSelectReceivePlace(string selectedIds, string valueFields)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var _data = service.GetData<Cat_ReceivePlaceEntity>(Common.DotNetToOracle(selectedIds), ConstantSql.hrm_Cat_SP_GET_RECEIVEPLACEBYIDs, ref status);

                #region Load template bao cao
                if (_data != null && status == NotificationType.Success.ToString())
                {
                    status = ExportService.Export(Guid.Empty, _data, valueFields.Split(','), null);
                }
                #endregion
            }
            return Json(status);
        }

        public ActionResult ExportAllReceivePlace([DataSourceRequest] DataSourceRequest request, Cat_ReceivePlaceSearchModel model)
        {
            model.SetPropertyValue("IsExport", true);
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            List<object> obj = new List<object>();
            obj.AddRange(new object[5]);
            obj[0] = model.ReceivePlace;
            obj[1] = model.Code;
            obj[2] = model.ModelType;
            obj[3] = 1;
            obj[4] = int.MaxValue - 1;
            var _data = service.GetData<Cat_ReceivePlaceEntity>(obj, ConstantSql.hrm_Cat_SP_GET_RECEIVEPLACE, ref status);

            if (_data != null && status == NotificationType.Success.ToString())
            {
                status = ExportService.Export(_data, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));
            }
            return Json(status);
        }

        #endregion
        #region Cat_model
        public ActionResult Cat_GetModelModel([DataSourceRequest] DataSourceRequest request, Cat_ModelSearchModel model)
        {
            var _service = new ActionService(UserLogin);
            string status = string.Empty;
            var _lstobj = new List<object>();
            _lstobj.AddRange(new object[9]);
            _lstobj[0] = model.ModelType;
            _lstobj[1] = model.ModelName;
            _lstobj[2] = model.ModelCode;
            _lstobj[3] = model.DateApplyFrom;
            _lstobj[4] = model.DateApplyTo;
            _lstobj[5] = model.DateExpireFrom;
            _lstobj[6] = model.DateExpireTo;
            _lstobj[7] = 1;
            _lstobj[8] = int.MaxValue - 1;
            var result = _service.GetData<Cat_ModelEntity>(_lstobj, ConstantSql.hrm_Cat_sp_get_CatModel, ref status).Translate<Cat_ModelModel>().ToList();
            if (!string.IsNullOrEmpty(model.Status))
            {
                if (model.Status == EnumDropDown.InActive.E_MODELACTIVE.ToString())
                {
                    result = result.Where(s => s.InActive == false || s.InActive == null).ToList();
                }
                else
                {
                    result = result.Where(s => s.InActive == true).ToList();
                }
            }

            #region xuất ra lưới.

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            #endregion
        }
        public ActionResult Cat_GetModelBYID(Guid modelid)
        {
            if (modelid != null)
            {
                ActionService actionserveice = new ActionService(UserLogin);
                string status = string.Empty;
                var result = actionserveice.GetByIdUseStore<Cat_ModelEntity>(modelid, ConstantSql.hrm_Cat_SP_GET_ModelByModelID, ref status);
                if (result != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetRateByOrgID(Guid? _orgStructureID)
        {
            if (_orgStructureID != null)
            {
                ActionService actionserveice = new ActionService(UserLogin);
                string status = string.Empty;
                var result = actionserveice.GetByIdUseStore<Cat_OrgStructureEntity>((Guid)_orgStructureID, ConstantSql.hrm_cat_sp_get_OrgStructureById, ref status);
                if (result != null)
                    return Json(result.SalaryDepartmentRate ?? 1, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult Cat_GetModelByModelType(string modelType)
        {
            ActionService actionserveice = new ActionService(UserLogin);
            string status = string.Empty;
            var result = actionserveice.GetData<Cat_ModelEntity>(modelType, ConstantSql.hrm_CAT_SP_GET_ModelByModelType, ref status);

            if (result != null)
                return Json(result, JsonRequestBehavior.AllowGet);
            return null;
        }


        public ActionResult SetDateEnd(DateTime DateFrom, int PayMent)
        {
            var dateend = DateFrom.AddMonths(PayMent - 1);
            var dateendformat = dateend.ToString("dd/MM/yyyy");
            return Json(dateendformat, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CatGetColorByModelID1(Guid ID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            string id = string.Empty;
            if (ID != null)
                id = Common.DotNetToOracle(ID.ToString());
            var result = baseService.GetData<PUR_ColorModelModel>(id, ConstantSql.hrm_Cat_SP_GET_ColorByModelID, UserLogin, ref status);
            foreach (var item in result)
            {
                item.Color = item.ColorCode + "_" + item.Color;
            }
            if (result != null)
                return Json(result, JsonRequestBehavior.AllowGet);
            return null;
        }
        public ActionResult CatGetColorByModelID([DataSourceRequest] DataSourceRequest request, Guid ModelID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            string id = string.Empty;
            if (ModelID != null)
                id = Common.DotNetToOracle(ModelID.ToString());
            var result = baseService.GetData<PUR_ColorModelModel>(id, ConstantSql.hrm_Cat_SP_GET_ColorByModelID, UserLogin, ref status);
            if (result != null)
                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return null;
        }

        public ActionResult ExportSelecteModel(string selectedIds, string valueFields)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var _data = service.GetData<Cat_ModelEntity>(Common.DotNetToOracle(selectedIds), ConstantSql.hrm_Cat_sp_get_CatModelByIDs, ref status);

                #region Load template bao cao
                if (_data != null && status == NotificationType.Success.ToString())
                {
                    foreach (var item in _data)
                    {
                        if (item.ModelType != null)
                            item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                    }
                    status = ExportService.Export(Guid.Empty, _data, valueFields.Split(','), null);
                }
                #endregion
            }
            return Json(status);
        }

        public ActionResult ExportAllModel([DataSourceRequest] DataSourceRequest request, Cat_ModelSearchModel model)
        {
            model.SetPropertyValue("IsExport", true);
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            var _lstobj = new List<object>();
            _lstobj.AddRange(new object[9]);
            _lstobj[0] = model.ModelType;
            _lstobj[1] = model.ModelName;
            _lstobj[2] = model.ModelCode;
            _lstobj[3] = model.DateApplyFrom;
            _lstobj[4] = model.DateApplyTo;
            _lstobj[5] = model.DateExpireFrom;
            _lstobj[6] = model.DateExpireTo;

            var _data = service.GetData<Cat_ModelEntity>(_lstobj, ConstantSql.hrm_Cat_sp_get_CatModel, ref status);

            if (_data != null && status == NotificationType.Success.ToString())
            {
                foreach (var item in _data)
                {
                    if (item.ModelType != null)
                        item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                status = ExportService.Export(_data, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));

            }
            return Json(status);
        }
        #endregion

        #region Cat_ReasonNotEligible
        public ActionResult ExportSelectReasonNotEligible(string selectedIds, string valueFields)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var _data = service.GetData<Cat_NameEntityEntity>(Common.DotNetToOracle(selectedIds), ConstantSql.hrm_CAT_SP_GET_NAMEENTITYBYIDs, ref status);

                #region Load template bao cao
                if (_data != null && status == NotificationType.Success.ToString())
                {
                    status = ExportService.Export(Guid.Empty, _data, valueFields.Split(','), null);
                }
                #endregion
            }
            return Json(status);
        }

        public ActionResult ExportAllReasonNotEligible([DataSourceRequest] DataSourceRequest request, Cat_NameEntityByKPISearchModel model)
        {
            //return GetListDataAndReturn<Cat_NameEntityModel, Cat_NameEntityEntity, Cat_NameEntityByKPISearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntityByKPI);
            model.SetPropertyValue("IsExport", true);
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            List<object> obj = new List<object>();
            obj.AddRange(new object[4]);
            obj[1] = model.NameEntityType;
            obj[2] = 1;
            obj[3] = int.MaxValue - 1;
            var _data = service.GetData<Cat_NameEntityEntity>(obj, ConstantSql.hrm_cat_sp_get_NameEntityByKPI, ref status);

            if (_data != null && status == NotificationType.Success.ToString())
            {
                status = ExportService.Export(_data, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));
            }
            return Json(status);
        }
        #endregion

        #region Cat_UnAllowCfgAmount
        public ActionResult GetUnAllowCfgAmountList([DataSourceRequest] DataSourceRequest request, Cat_UnAllowCfgAmountSearchModel model)
        {
            return GetListDataAndReturn<Cat_UnAllowCfgAmountModel, Cat_UnAllowCfgAmountEntity, Cat_UnAllowCfgAmountSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_UnAllowCfgAmount);
        }
        public ActionResult ExportUnAllowCfgAmountSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_UnAllowCfgAmountEntity, Cat_UnAllowCfgAmountModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_UnAllowCfgAmountByIds);
        }
        public ActionResult ExportAllUnAllowCfgAmountList([DataSourceRequest] DataSourceRequest request, Cat_UnAllowCfgAmountSearchModel model)
        {
            return ExportAllAndReturn<Cat_UnAllowCfgAmountEntity, Cat_UnAllowCfgAmountModel, Cat_UnAllowCfgAmountSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_UnAllowCfgAmount);
        }
        #endregion


        #region Cat_DataGroup
        public ActionResult GetDataGroupList([DataSourceRequest] DataSourceRequest request, Cat_DataGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_DataGroupModel, Cat_DataGroupEntity, Cat_DataGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DataGroup);
        }
        public ActionResult ExportDataGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_DataGroupEntity, Cat_DataGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_DataGroupByIds);
        }
        public ActionResult ExportAllDataGroupList([DataSourceRequest] DataSourceRequest request, Cat_DataGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_DataGroupEntity, Cat_DataGroupModel, Cat_DataGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DataGroup);
        }
        #endregion
        #region Cat_DataGroupDetail

        public ActionResult ExportDataGroupDetailSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_DataGroupDetailEntity, Cat_DataGroupDetailModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_DataGroupByIds);
        }
        public ActionResult GetDataGroupDetailByDataGroupID([DataSourceRequest] DataSourceRequest request, Guid dataGroupID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(dataGroupID);
            var result = baseService.GetData<Cat_DataGroupDetailModel>(objs, ConstantSql.hrm_cat_sp_get_DataGroupDetailByDTGroupID, UserLogin, ref status);
            if (result != null)
                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return null;
        }

        public JsonResult GetMultiDataGroup(string text)
        {
            return GetDataForControl<Cat_DataGroupMultiModel, Cat_DataGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_DataGroup_multi);
        }

        public JsonResult GetMultiMasterDataGroup(string text)
        {
            return GetDataForControl<Cat_MasterDataGroupMultiModel, Cat_MasterDataGroupMultiModel>(text, ConstantSql.hrm_cat_sp_get_MasterDataGroup_multi);
        }
        #endregion

        #region Cat_TrainLevel
        public ActionResult GetTrainLevelList([DataSourceRequest] DataSourceRequest request, Cat_TrainLevelSearchModel model)
        {
            return GetListDataAndReturn<Cat_TrainLevelModel, Cat_TrainLevelEntity, Cat_TrainLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TrainLevel);
        }
        public ActionResult ExportTrainLevelSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_TrainLevelEntity, Cat_TrainLevelModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_TrainLevelByIds);
        }
        public ActionResult ExportAllTrainLevelList([DataSourceRequest] DataSourceRequest request, Cat_TrainLevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_TrainLevelEntity, Cat_TrainLevelModel, Cat_TrainLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TrainLevel);
        }

        public JsonResult GetMultiTrainLevel(string text)
        {
            return GetDataForControl<Cat_TrainLevelMultiModel, Cat_TrainLevelMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TrainLevel_Multi);
        }
        #endregion

        #region Cat_TrainCategory
        public ActionResult GetTrainCategoryList([DataSourceRequest] DataSourceRequest request, Cat_TrainCategorySearchModel model)
        {
            return GetListDataAndReturn<Cat_TrainCategoryModel, Cat_TrainCategoryEntity, Cat_TrainCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TrainCategory);
        }
        public ActionResult ExportTrainCategorySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_TrainCategoryEntity, Cat_TrainCategoryModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_TrainCategoryByIds);
        }
        public ActionResult ExportAllTrainCategoryList([DataSourceRequest] DataSourceRequest request, Cat_TrainCategorySearchModel model)
        {
            return ExportAllAndReturn<Cat_TrainCategoryEntity, Cat_TrainCategoryModel, Cat_TrainCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TrainCategory);
        }

        public JsonResult GetMultiTrainCategory(string text)
        {
            return GetDataForControl<Cat_TrainCategoryMultiModel, Cat_TrainCategoryMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TrainCategory_Multi);
        }
        #endregion

        #region Cat_ApprovedGrade
        public ActionResult GetListApprovedGrade([DataSourceRequest] DataSourceRequest request, Cat_ApprovedGradeSearchModel model)
        {
            return GetListDataAndReturn<Cat_ApprovedGradeModel, Cat_ApprovedGradeEntity, Cat_ApprovedGradeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ApprovedGrade);
        }

        public ActionResult GetListApprovedGradeDetail([DataSourceRequest] DataSourceRequest request, Cat_ApprovedGradeDetailSearchModel model, string type)
        {
            request.Filters = new List<IFilterDescriptor>();
            if (type == ConceptType.E_EACH_PROFILE.ToString())
            {
                request.Filters.Add(new FilterDescriptor { Member = "ProfileID", Operator = FilterOperator.IsNotNull });
            }
            else if (type == ConceptType.E_EACH_ORG.ToString())
            {
                request.Filters.Add(new FilterDescriptor { Member = "DepartmentID", Operator = FilterOperator.IsNotNull });
            }
            else if (type == ConceptType.E_EACH_POSITION.ToString())
            {
                request.Filters.Add(new FilterDescriptor { Member = "PositionID", Operator = FilterOperator.IsNotNull });
            }

            return GetListDataAndReturn<Cat_ApprovedGradeDetailModel, Cat_ApprovedGradeDetailEntity, Cat_ApprovedGradeDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ApprovedGradeDetail);
        }
        #endregion

        #region Cat_Owner
        public ActionResult GetOwnerList([DataSourceRequest] DataSourceRequest request, Cat_OwnerSearchModel model)
        {
            return GetListDataAndReturn<Cat_OwnerModel, Cat_OwnerEntity, Cat_OwnerSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Owner);
        }
        public ActionResult ExportOwnerSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_OwnerEntity, Cat_OwnerModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_OwnerByIds);
        }
        public ActionResult ExportAllOwnerList([DataSourceRequest] DataSourceRequest request, Cat_OwnerSearchModel model)
        {
            return ExportAllAndReturn<Cat_OwnerEntity, Cat_OwnerModel, Cat_OwnerSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Owner);
        }
        public JsonResult GetMultiOwner(string text)
        {
            return GetDataForControl<Cat_OwnerMultiModel, Cat_OwnerMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Owner_Multi);
        }

        public JsonResult GetMultiFunction(string text)
        {
            return GetDataForControl<Cat_OwnerMultiModel, Cat_OwnerMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Function_Multi);
        }


        public JsonResult GetBudgetOwnerCascading(Guid functionID, string provinceFilter)
        {
            var result = new List<Fin_PurchaseRequestModel>();
            string status = string.Empty;
            if (functionID != Guid.Empty)
            {
                var service = new Cat_OwnerServices();
                result = service.GetData<Fin_PurchaseRequestModel>(functionID, ConstantSql.hrm_cat_sp_get_BudgetByFunctionId, UserLogin, ref status);
                //if (!string.IsNullOrEmpty(provinceFilter))
                //{
                //    var rs = result.Where(s => s.ProvinceName != null && s.ProvinceName.ToLower().Contains(provinceFilter.ToLower())).ToList();

                //    return Json(rs, JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetChannelCascading(Guid budgetOwnerID, string provinceFilter)
        {
            var result = new List<Fin_PurchaseRequestModel>();
            string status = string.Empty;
            if (budgetOwnerID != Guid.Empty)
            {
                var service = new Cat_OwnerServices();
                result = service.GetData<Fin_PurchaseRequestModel>(budgetOwnerID, ConstantSql.hrm_cat_sp_get_ChannelByBudgetOwnerId, UserLogin, ref status);
                //if (!string.IsNullOrEmpty(provinceFilter))
                //{
                //    var rs = result.Where(s => s.ProvinceName != null && s.ProvinceName.ToLower().Contains(provinceFilter.ToLower())).ToList();

                //    return Json(rs, JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCostCentreCascading(Guid budgetOwnerID, string provinceFilter)
        {
            var result = new List<Fin_PurchaseRequestModel>();
            string status = string.Empty;
            if (budgetOwnerID != Guid.Empty)
            {
                var serivces = new ActionService(UserLogin);
                var entity = serivces.GetByIdUseStore<Fin_PurchaseRequestModel>(budgetOwnerID, ConstantSql.hrm_cat_sp_get_OwnerByIds, ref status);
                var cateService = new Cat_CateCodeServices();
                var lstObj = new List<object>();
                lstObj.Add(null);
                lstObj.Add(null);
                lstObj.Add(1);
                lstObj.Add(int.MaxValue - 1);
                var lstCate = cateService.GetData<Cat_CateCodeModel>(lstObj, ConstantSql.hrm_cat_sp_get_CateCode, UserLogin, ref status);
                if (entity != null)
                {
                    if (entity.BudgetOwnerName == "EUCERIN")
                    {
                        var lstCateCode = lstCate.Where(s => s.CateCodeType == entity.BudgetOwnerName);
                        return Json(lstCateCode, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var lstCateCodes = lstCate.Where(s => s.CateCodeType != "EUCERIN");
                        return Json(lstCateCodes, JsonRequestBehavior.AllowGet);
                    }

                }
                //if (!string.IsNullOrEmpty(provinceFilter))
                //{
                //    var rs = result.Where(s => s.ProvinceName != null && s.ProvinceName.ToLower().Contains(provinceFilter.ToLower())).ToList();

                //    return Json(rs, JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemCascading(Guid budgetOwnerID, string provinceFilter)
        {
            var result = new List<Fin_PurchaseRequestModel>();
            string status = string.Empty;
            if (budgetOwnerID != Guid.Empty)
            {
                var itemServices = new Cat_PurchaseItemsServices();
                var lstObj = new List<object>();
                lstObj.Add(null);
                lstObj.Add(1);
                lstObj.Add(int.MaxValue - 1);
                var lstPurchaseItem = itemServices.GetData<Cat_PurchaseItemsModel>(lstObj, ConstantSql.hrm_cat_sp_get_PurchaseItems, UserLogin, ref status);
                lstPurchaseItem.Where(s => s.OwnerID == budgetOwnerID);
                return Json(lstPurchaseItem, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectCascading(Guid budgetOwnerID, string provinceFilter)
        {
            var result = new List<Cat_MasterProjectModel>();
            string status = string.Empty;
            if (budgetOwnerID != Guid.Empty)
            {
                var serivces = new ActionService(UserLogin);
                var entity = serivces.GetByIdUseStore<Fin_PurchaseRequestModel>(budgetOwnerID, ConstantSql.hrm_cat_sp_get_OwnerByIds, ref status);

                var ProjectServices = new Cat_MasterProjectServices();
                var lstObj = new List<object>();
                lstObj.Add(null);
                lstObj.Add(1);
                lstObj.Add(int.MaxValue - 1);
                var lstMasterProject = ProjectServices.GetData<Cat_MasterProjectModel>(lstObj, ConstantSql.hrm_cat_sp_get_MasterProject, UserLogin, ref status);
                if (entity != null)
                {
                    if (entity.BudgetOwnerName != "EUCERIN")
                    {
                        var lstMasterProjects = lstMasterProject.Where(s => s.Type != "EUCERIN");
                        return Json(lstMasterProjects, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion
        #region Cat_CateCode

        public ActionResult GetCateCodeList([DataSourceRequest] DataSourceRequest request, Cat_CateCodeSearchModel model)
        {
            return GetListDataAndReturn<Cat_CateCodeModel, Cat_CateCodeEntity, Cat_CateCodeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CateCode);
        }
        public ActionResult ExportCateCodeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CateCodeEntity, Cat_CateCodeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CateCodeByIds);
        }
        public ActionResult ExportAllCateCodeList([DataSourceRequest] DataSourceRequest request, Cat_CateCodeSearchModel model)
        {
            return ExportAllAndReturn<Cat_CateCodeEntity, Cat_CateCodeModel, Cat_CateCodeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CateCode);
        }



        #endregion
        #region Cat_MasterProject
        public ActionResult GetMasterProjectList([DataSourceRequest] DataSourceRequest request, Cat_MasterProjectSearchModel model)
        {
            return GetListDataAndReturn<Cat_MasterProjectModel, Cat_MasterProjectEntity, Cat_MasterProjectSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_MasterProject);
        }
        public ActionResult ExportMasterProjectSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_MasterProjectEntity, Cat_MasterProjectModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_MasterProjectByIds);
        }
        public ActionResult ExportAllMasterProjectList([DataSourceRequest] DataSourceRequest request, Cat_MasterProjectSearchModel model)
        {
            return ExportAllAndReturn<Cat_MasterProjectEntity, Cat_MasterProjectModel, Cat_MasterProjectSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_MasterProject);
        }
        #endregion

        #region Cat_RelativeType
        public JsonResult GetMultiRelativeType(string text)
        {
            return GetDataForControl<CatRelativeTypeModel, Cat_RelativeTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_RelativeType_multi);
        }
        #endregion

        #region Cat_DisciplinedTypes
        public JsonResult GetMultiDisciplinedTypes(string text)
        {
            return GetDataForControl<Cat_DisciplinedTypesMultiModel, Cat_DisciplinedTypesMultiEntity>(text, ConstantSql.hrm_cat_sp_get_DisciplinedTypes_multi);
        }
        public ActionResult GetDisciplinedTypesList([DataSourceRequest] DataSourceRequest request, Cat_DisciplinedTypesSearchModel model)
        {
            return GetListDataAndReturn<Cat_DisciplinedTypesModel, Cat_DisciplinedTypesEntity, Cat_DisciplinedTypesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DisciplinedTypes);
        }

        public ActionResult ExportAllDisciplinedTypesList([DataSourceRequest] DataSourceRequest request, Cat_DisciplinedTypesSearchModel model)
        {
            return ExportAllAndReturn<Cat_DisciplinedTypesEntity, Cat_DisciplinedTypesModel, Cat_DisciplinedTypesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DisciplinedTypes);
        }

        #endregion
        #region Cat_GetMultiSubjectGroup
        public JsonResult GetMultiSubjectGroup(string text)
        {

            return GetDataForControl<CatSubjectGroupMultiModel, CatSubjectGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SubjectGroup_multi);
        }
        #endregion

        #region Cat_Survey
        public JsonResult GetMultiSurvey(string text)
        {
            return GetDataForControl<Cat_SurveyMultiModel, Cat_SurveyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Survey_multi);
        }

        public JsonResult GetMultiSurveyLeave(string text)
        {
            return GetDataForControl<Cat_SurveyMultiModel, Cat_SurveyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SurveyLeave_multi);
        }
        #endregion

        #region Cat_SurveyQuestionType
        public JsonResult GetMultiSurveyQuestionType(string text)
        {
            return GetDataForControl<Cat_SurveyQuestionTypeMultiModel, Cat_SurveyQuestionTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SurveyQuestionType_multi);
        }
        #endregion

        public ActionResult GetMultiShiftByOrdernumber(string text)
        {
            //[To.Le][26/08/2016][New Func][0072174]Sắp xếp ca làm việc theo OrderNumber 
            List<Cat_ShiftMultiEntity> lstShitItemEntity = new List<Cat_ShiftMultiEntity>();
            string status = string.Empty;
            var services = new ActionService(LanguageCode);
            var obj = new List<object>();
            obj.AddRange(new object[3]);
            obj[0] = text;
            obj[1] = 1;
            obj[2] = int.MaxValue - 1;
            var lstShift = baseService.GetData<Cat_ShiftMultiEntity>(obj, ConstantSql.hrm_cat_sp_get_Shift_multiByOrderNumber, UserLogin, ref status);
            if (lstShift != null)
            {
                var lstOrdenumber = lstShift.Select(s => s.OrderNumber == null).ToList();
                if (lstOrdenumber.Count == 0)
                {
                    lstShift = lstShift.OrderBy(s => s.OrderNumber).ToList();
                }
            }

            return Json(lstShift, JsonRequestBehavior.AllowGet);
        }
        #region Cat_Shift
        public JsonResult GetMultiShift(string text)
        {
            return GetDataForControl<CatShiftMultiModel, Cat_ShiftMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Shift_multi);
        }

        public JsonResult GetMultiShiftDetail(string text)
        {

            return GetDataForControl<Cat_ShiftDetailMultiModel, Cat_ShiftDetailMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ShiftDetail_multi);
        }
        public ActionResult GetShiftItemByShiftIDList([DataSourceRequest] DataSourceRequest request, Guid ShiftID)
        {
            List<Cat_ShiftItemEntity> lstShitItemEntity = new List<Cat_ShiftItemEntity>();
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ShiftID);
            var result = baseService.GetData<Cat_ShiftItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ShiftItemByShiftID, UserLogin, ref status);
            if (result != null)
            {

                foreach (var item in result)
                {
                    DateTime temp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.Intime.Value.Hour, item.Intime.Value.Minute, item.Intime.Value.Second);
                    item.From = temp.AddHours(item.CoFrom);
                    //  item.To = DateTime.Now;
                    item.To = temp.AddHours(item.CoTo);
                    lstShitItemEntity.Add(item);
                }
            }
            return Json(lstShitItemEntity.ToDataSourceResult(request));
        }

        public ActionResult GetShiftOvertimeItemByShiftIDList([DataSourceRequest] DataSourceRequest request, Guid ShiftID)
        {
            List<Cat_ShiftItemEntity> lstShitItemEntity = new List<Cat_ShiftItemEntity>();
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ShiftID);
            var result = baseService.GetData<Cat_ShiftItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ShiftItemOvertimeByShiftID, UserLogin, ref status);
            if (result != null)
            {
                foreach (var item in result)
                {
                    DateTime temp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.Intime.Value.Hour, item.Intime.Value.Minute, item.Intime.Value.Second);
                    item.From = temp.AddHours(item.CoFrom);
                    //  item.To = DateTime.Now;
                    item.To = temp.AddHours(item.CoTo);
                    lstShitItemEntity.Add(item);
                }
            }
            return Json(lstShitItemEntity.ToDataSourceResult(request));
        }
        #endregion

        #region ElementType
        public JsonResult GetElementType(string text)
        {

            return GetDataForControl<Sal_CostCentreSalElementTypeMultiModel, Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi);

        }

        public JsonResult GetElementTypePaidLeave(string text)
        {
            if (text == null || text == string.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                //text = "Paid";
                var listEntity = baseService.GetData<Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi, UserLogin, ref status);

                if (listEntity != null)
                {
                    listEntity = listEntity.Where(s => s.ElementCode == Common.PaidLeaveAndGoodHealth).ToList();
                    List<Sal_CostCentreSalElementTypeMultiModel> listModel = listEntity.Translate<Sal_CostCentreSalElementTypeMultiModel>();
                    return Json(listModel, JsonRequestBehavior.AllowGet);
                }
            }
            return GetDataForControl<Sal_CostCentreSalElementTypeMultiModel, Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi);
        }

        public JsonResult GetElementTypeOvertimeTempAllowance(string text)
        {
            if (text == null || text == string.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                var listEntity = baseService.GetData<Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi, UserLogin, ref status);

                if (listEntity != null)
                {
                    listEntity = listEntity.Where(s => s.ElementCode == Common.OvertimeTempAmount).ToList();
                    List<Sal_CostCentreSalElementTypeMultiModel> listModel = listEntity.Translate<Sal_CostCentreSalElementTypeMultiModel>();
                    return Json(listModel, JsonRequestBehavior.AllowGet);
                }
            }
            return GetDataForControl<Sal_CostCentreSalElementTypeMultiModel, Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi);
        }

        public JsonResult GetElementTypeBonusEvaluation(string text)
        {
            if (text == null || text == string.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                var listEntity = baseService.GetData<Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi, UserLogin, ref status);

                if (listEntity != null)
                {
                    listEntity = listEntity.Where(s => s.ElementCode == Common.BonusEvaluation_PT).ToList();
                    List<Sal_CostCentreSalElementTypeMultiModel> listModel = listEntity.Translate<Sal_CostCentreSalElementTypeMultiModel>();
                    return Json(listModel, JsonRequestBehavior.AllowGet);
                }
            }
            return GetDataForControl<Sal_CostCentreSalElementTypeMultiModel, Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi);
        }
        #endregion

        #region Thưởng nghỉ lễ
        public JsonResult GetElementTypeHolidayBonus(string text)
        {
            if (text == null || text == string.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                var listEntity = baseService.GetData<Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi, UserLogin, ref status);

                if (listEntity != null)
                {
                    listEntity = listEntity.Where(s => s.ElementCode == Common.AmountHolidayBonus).ToList();
                    List<Sal_CostCentreSalElementTypeMultiModel> listModel = listEntity.Translate<Sal_CostCentreSalElementTypeMultiModel>();
                    return Json(listModel, JsonRequestBehavior.AllowGet);
                }
            }
            return GetDataForControl<Sal_CostCentreSalElementTypeMultiModel, Cat_ElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Element_Multi);
        }
        #endregion

        #region Cat_CostCentreDistribution

        [HttpPost]
        public ActionResult GetCostCentreDistribution([DataSourceRequest] DataSourceRequest request, Cat_CostCentreDistributionSearchModel model)
        {
            return GetListDataAndReturn<Cat_CostCentreDistributionModel, Cat_CostCentreDistributionEntity, Cat_CostCentreDistributionSearchModel>
                (request, model, ConstantSql.hrm_Cat_sp_get_CostCentreDistribution);
        }

        #endregion

        #region cat_Orgstructure
        public JsonResult GetRegionByOrgStructureID(Guid ID, Guid? profileID, string Check)
        {
            if (Check == null)
            {
                ActionService Services = new ActionService(UserLogin);
                string status = string.Empty;
                var DataOrg = Services.GetByIdUseStore<Cat_OrgStructureEntity>(ID, ConstantSql.hrm_cat_sp_get_OrgStructureById, ref status);
                Hre_ProfileEntity DataProfile;
                Cat_RegionMultiEntity DataRegion;
                if (DataOrg.RegionID != null)
                {
                    if (profileID != null)
                    {
                        status = string.Empty;
                        DataProfile = Services.GetByIdUseStore<Hre_ProfileEntity>(profileID.Value, ConstantSql.hrm_hr_sp_get_ProfileById, ref status);
                        if (DataProfile.RegionID == Guid.Empty)
                        {
                            DataRegion = Services.GetByIdUseStore<Cat_RegionMultiEntity>(DataOrg.RegionID.Value, ConstantSql.hrm_cat_sp_get_RegionById, ref status);
                            return Json(DataRegion, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            DataRegion = Services.GetByIdUseStore<Cat_RegionMultiEntity>(DataProfile.RegionID.Value, ConstantSql.hrm_cat_sp_get_RegionById, ref status);
                            return Json(DataRegion, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        DataRegion = Services.GetByIdUseStore<Cat_RegionMultiEntity>(DataOrg.RegionID.Value, ConstantSql.hrm_cat_sp_get_RegionById, ref status);
                        return Json(DataRegion, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(GetDataForControl<CatRegionMultiModel, Cat_RegionMultiEntity>("", ConstantSql.hrm_cat_sp_get_Region_multi), JsonRequestBehavior.AllowGet);

            }
            return Json(null);
        }

        public JsonResult GetRegionByWorkPlaceID(Guid ID, Guid? profileID, string Check)
        {
            if (Check == null)
            {
                ActionService Services = new ActionService(UserLogin);
                string status = string.Empty;
                var DataWpl = Services.GetByIdUseStore<Cat_WorkPlaceEntity>(ID, ConstantSql.hrm_cat_sp_get_WorkPlaceById, ref status);
                Hre_ProfileEntity DataProfile;
                Cat_RegionMultiEntity DataRegion;
                if (DataWpl.RegionID != null)
                {
                    if (profileID != null)
                    {
                        DataProfile = Services.GetByIdUseStore<Hre_ProfileEntity>(profileID.Value, ConstantSql.hrm_hr_sp_get_ProfileById, ref status);
                        if (DataProfile.RegionID == Guid.Empty)
                        {
                            DataRegion = Services.GetByIdUseStore<Cat_RegionMultiEntity>(DataWpl.RegionID.Value, ConstantSql.hrm_cat_sp_get_RegionById, ref status);
                            return Json(DataRegion, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            DataRegion = Services.GetByIdUseStore<Cat_RegionMultiEntity>(DataProfile.RegionID.Value, ConstantSql.hrm_cat_sp_get_RegionById, ref status);
                            return Json(DataRegion, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        DataRegion = Services.GetByIdUseStore<Cat_RegionMultiEntity>(DataWpl.RegionID.Value, ConstantSql.hrm_cat_sp_get_RegionById, ref status);
                        return Json(DataRegion, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(GetDataForControl<CatRegionMultiModel, Cat_RegionMultiEntity>("", ConstantSql.hrm_cat_sp_get_Region_multi), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null);
        }

        //Son.Vo - 20160708 - 0070174 - hiển thị công ty khi chọn phòng ban
        [HttpPost]
        public ActionResult GetOrgStructureDataByID(string orgstructureID)
        {
            var profileServices = new Hre_ProfileServices();
            string status = string.Empty;
            var orgID = Guid.Empty;
            if (string.IsNullOrEmpty(orgstructureID) || orgstructureID.IndexOf(',') > 0)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(orgstructureID))
            {
                orgID = Common.ConvertToGuid(orgstructureID);
            }
            var orgEntity = profileServices.GetOrgStructureDataByID(orgID);

            if (orgEntity != null)
            {
                return Json(orgEntity, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetOrgHistoryByOrgID([DataSourceRequest] DataSourceRequest request, Guid? ogrID)
        {
            if (ogrID != null && ogrID != Guid.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                var result = baseService.GetData<Cat_OrgHistoryEntity>(Common.DotNetToOracle(ogrID.ToString()), ConstantSql.hrm_cat_sp_get_OrgHistoryByOrgID, UserLogin, ref status);
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }
        #endregion

        #region Cat_OrgStructureType


        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng OrgStructureType (Cat_OrgStructureType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrgStructureTypeList([DataSourceRequest] DataSourceRequest request, CatOrgStructureTypeSearchModel model)
        {
            return GetListDataAndReturn<CatOrgStructureTypeModel, Cat_OrgStructureTypeEntity, CatOrgStructureTypeSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_OrgStructureType);
        }

        public JsonResult GetMultiOrgStructureType(string text)
        {
            return GetDataForControl<CatOrgStructureTypeMultiModel, Cat_OrgStructureTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_OrgStructureType_multi);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho OrgStructureType(OrgStructureType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllOrgStructureTypeList([DataSourceRequest] DataSourceRequest request, CatOrgStructureTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_OrgStructureTypeEntity, CatOrgStructureTypeModel, CatOrgStructureTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_OrgStructureType);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn OrgStructureType (OrgStructureType) theo điều kiện tìm kiếm
        public ActionResult ExportOrgStructureTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_OrgStructureTypeEntity, CatOrgStructureTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }

        #endregion

        #region Cat_OrgStructure
        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng OrgStructure (Cat_OrgStructure)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrgStructureList([DataSourceRequest] DataSourceRequest request, CatOrgStructureSearchModelV3 model)
        {

            int page = request.Page;
            int pagesize = request.PageSize;
            if (model != null && model.ExportID != Guid.Empty)
            {
                page = 1;
                pagesize = int.MaxValue - 1;
            }
            string status = string.Empty;
            ActionService _ser = new ActionService(UserLogin);

            #region Tạo template CatOrgStructureModel
            DataTable table = new DataTable("CatOrgStructureModel");
            table.Columns.Add("CodeOrgStructure");
            table.Columns.Add("MailingAddress");
            table.Columns.Add("FileAttach");
            table.Columns.Add("FreeService");
            table.Columns.Add("OrgFullName");
            table.Columns.Add("Email");
            table.Columns.Add("Website");
            table.Columns.Add("Fax");
            table.Columns.Add("Phone");
            table.Columns.Add("OrderOrg");
            table.Columns.Add("OrgStructureName");
            table.Columns.Add("OrgStructureID");
            table.Columns.Add("OrgStructureNameEN");
            table.Columns.Add("Code");
            //table.Columns.Add("IsRoot");
            table.Columns.Add("Description");
            table.Columns.Add("TypeID");
            table.Columns.Add("ParentID");
            table.Columns.Add("OrderNumber");
            table.Columns.Add("OrgStructureTypeName");
            table.Columns.Add("Icon");
            table.Columns.Add("OrgStructureTypeID");
            table.Columns.Add("OrgStructureParentName");
            table.Columns.Add("ProfileCount");
            table.Columns.Add("ProfileIsWorking");
            table.Columns.Add("ExportID");
            table.Columns.Add("FEMALE");
            table.Columns.Add("FEMALEEMP");
            table.Columns.Add("MALE");
            table.Columns.Add("MALEEMP");
            table.Columns.Add("GroupCostCentreID");
            table.Columns.Add("CostCentreName");
            //table.Columns.Add("IsShow");
            table.Columns.Add("OrgStructure_ID");
            table.Columns.Add("RegionName");
            table.Columns.Add("RegionID");
            table.Columns.Add("ServicesType");
            table.Columns.Add("ContractFrom", typeof(DateTime));
            table.Columns.Add("ContractTo", typeof(DateTime));
            table.Columns.Add("BillingCompanyName");
            table.Columns.Add("BillingAddress");
            table.Columns.Add("TaxCode");
            table.Columns.Add("DurationPay");
            table.Columns.Add("RecipientInvoice");
            table.Columns.Add("TelePhone");
            table.Columns.Add("CellPhone");
            table.Columns.Add("DescriptionInfo");
            table.Columns.Add("EmailInfo");
            table.Columns.Add("OrgMoreInforID");
            table.Columns.Add("HrPlanHC");
            table.Columns.Add("E_COMPANY");
            table.Columns.Add("E_BRANCH");
            table.Columns.Add("E_UNIT");
            table.Columns.Add("E_DIVISION");
            table.Columns.Add("E_DEPARTMENT");
            table.Columns.Add("E_TEAM");
            table.Columns.Add("E_SECTION");
            table.Columns.Add("E_OU_L8");
            table.Columns.Add("E_OU_L9");
            table.Columns.Add("E_OU_L10");
            table.Columns.Add("E_OU_L11");
            table.Columns.Add("E_OU_L12");
            table.Columns.Add("E_COMPANY_CODE");
            table.Columns.Add("E_BRANCH_CODE");
            table.Columns.Add("E_UNIT_CODE");
            table.Columns.Add("E_DIVISION_CODE");
            table.Columns.Add("E_DEPARTMENT_CODE");
            table.Columns.Add("E_TEAM_CODE");
            table.Columns.Add("E_SECTION_CODE");
            table.Columns.Add("E_OU_L8_CODE");
            table.Columns.Add("E_OU_L9_CODE");
            table.Columns.Add("E_OU_L10_CODE");
            table.Columns.Add("E_OU_L11_CODE");
            table.Columns.Add("E_OU_L12_CODE");
            table.Columns.Add("ContactName");
            table.Columns.Add("Area");
            table.Columns.Add("Position");
            table.Columns.Add("TelephoneInforContract");
            table.Columns.Add("CellPhoneInforContract");
            table.Columns.Add("EmailInforContract");
            table.Columns.Add("DescriptionInforContract");
            #endregion
            if (model != null && model.IsCreateTemplate)
            {

                var path = Common.GetPath("Templates");
                ExportService exportService = new ExportService();

                ConfigExport cfgExport = new ConfigExport()
                {
                    Object = table,
                    FileName = "CatOrgStructureModel",
                    OutPutPath = path,
                    // HeaderInfo = listHeaderInfo,
                    DownloadPath = Hrm_Main_Web + "Templates",
                    IsDataTable = true
                };
                var str = exportService.CreateTemplate(cfgExport);
                return Json(str);
            }

            //[04/11/2015][to.le][Modify][0059525]
            //Thêm diều kiện tìm kiếm theo số thứ tự và phòng ban cha(OrderNumber, ParentID)
            //var _lsobj = new List<object>();
            //_lsobj.AddRange(new object[7]);
            //_lsobj[0] = model.OrgStructureName;
            //_lsobj[1] = model.Code;
            //_lsobj[2] = model.OrgStructureTypeID;
            //_lsobj[3] = model.OrderNumber;
            //_lsobj[4] = model.ParentID;
            //_lsobj[5] = page;
            //_lsobj[6] = pagesize;
            //var result = _ser.GetData<Cat_OrgStructureEntity>(_lsobj, ConstantSql.hrm_cat_sp_get_AllOrgStructureList, ref status).Translate<CatOrgStructureModel>();
            var result = GetListData<Cat_OrgStructureEntity, Cat_OrgStructureEntity, CatOrgStructureSearchModelV3>(request, model, ConstantSql.hrm_cat_sp_get_AllOrgStructureList, ref status);
            #region [19/10/2015][Phuc.Nguyen][raisetask][0058579] lấy thêm enum tạo mẫu báo cáo
            var objOrgMoreInforContract = new List<object>();
            objOrgMoreInforContract.AddRange(new object[2]);
            objOrgMoreInforContract[0] = 1;
            objOrgMoreInforContract[1] = int.MaxValue - 1;
            var lstOrgMoreInforContract = _ser.GetData<Cat_OrgMoreInforContractEntity>(objOrgMoreInforContract, ConstantSql.hrm_cat_sp_get_OrgMoreInforContract, ref status);
            #endregion
            foreach (var item in result)
            {
                DataRow dr = table.NewRow();
                dr["CodeOrgStructure"] = item.CodeOrgStructure;
                dr["MailingAddress"] = item.MailingAddress;
                dr["FileAttach"] = item.FileAttach;
                dr["FreeService"] = item.FreeService;
                dr["OrgFullName"] = item.OrgFullName;
                dr["Email"] = item.Email;
                dr["Website"] = item.Website;
                dr["Fax"] = item.Fax;
                dr["Phone"] = item.Phone;
                if (item.OrderOrg != null)
                    dr["OrderOrg"] = item.OrderOrg;
                dr["OrgStructureName"] = item.OrgStructureName;
                dr["OrgStructureID"] = item.OrgStructureID;
                dr["OrgStructureNameEN"] = item.OrgStructureNameEN;
                dr["Code"] = item.Code;
                dr["Description"] = item.Description;
                if (item.TypeID != null)
                    dr["TypeID"] = item.TypeID;
                if (item.ParentID != null)
                    dr["ParentID"] = item.ParentID;
                dr["OrderNumber"] = item.OrderNumber;
                dr["OrgStructureTypeName"] = item.OrgStructureTypeName;
                dr["Icon"] = item.Icon;
                if (item.OrgStructureTypeID != null)
                    dr["OrgStructureTypeID"] = item.OrgStructureTypeID;
                dr["OrgStructureParentName"] = item.OrgStructureParentName;
                dr["ProfileCount"] = item.ProfileCount;
                dr["ProfileIsWorking"] = item.ProfileIsWorking;
                dr["FEMALE"] = item.FEMALE;
                dr["FEMALEEMP"] = item.FEMALEEMP;
                dr["MALE"] = item.MALE;
                dr["MALEEMP"] = item.MALEEMP;
                dr["GroupCostCentreID"] = item.GroupCostCentreID;
                dr["CostCentreName"] = item.CostCentreName;
                dr["RegionName"] = item.RegionName;
                if (item.RegionID != null)
                    dr["RegionID"] = item.RegionID;
                dr["ServicesType"] = item.ServicesType;
                if (item.ContractFrom != null)
                    dr["ContractFrom"] = item.ContractFrom;
                if (item.ContractTo != null)
                    dr["ContractTo"] = item.ContractTo;
                dr["BillingCompanyName"] = item.BillingCompanyName;
                dr["BillingAddress"] = item.BillingAddress;
                dr["TaxCode"] = item.TaxCode;
                dr["DurationPay"] = item.DurationPay;
                dr["RecipientInvoice"] = item.RecipientInvoice;
                dr["TelePhone"] = item.TelePhone;
                dr["CellPhone"] = item.CellPhone;
                dr["DescriptionInfo"] = item.DescriptionInfo;
                dr["EmailInfo"] = item.EmailInfo;
                if (item.OrgMoreInforID != null)
                    dr["OrgMoreInforID"] = item.OrgMoreInforID;
                dr["HrPlanHC"] = item.HrPlanHC;
                dr["E_COMPANY"] = item.E_COMPANY;
                dr["E_BRANCH"] = item.E_BRANCH;
                dr["E_UNIT"] = item.E_UNIT;
                dr["E_DIVISION"] = item.E_DIVISION;
                dr["E_DEPARTMENT"] = item.E_DEPARTMENT;
                dr["E_SECTION"] = item.E_SECTION;
                dr["E_TEAM"] = item.E_TEAM;
                dr["E_OU_L8"] = item.E_OU_L8;
                dr["E_OU_L9"] = item.E_OU_L9;
                dr["E_OU_L10"] = item.E_OU_L10;
                dr["E_OU_L11"] = item.E_OU_L11;
                dr["E_OU_L12"] = item.E_OU_L12;
                dr["E_COMPANY_CODE"] = item.E_COMPANY_CODE;
                dr["E_BRANCH_CODE"] = item.E_BRANCH_CODE;
                dr["E_UNIT_CODE"] = item.E_UNIT_CODE;
                dr["E_DIVISION_CODE"] = item.E_DIVISION_CODE;
                dr["E_DEPARTMENT_CODE"] = item.E_DEPARTMENT_CODE;
                dr["E_TEAM_CODE"] = item.E_TEAM_CODE;
                dr["E_SECTION_CODE"] = item.E_SECTION_CODE;
                dr["E_OU_L8_CODE"] = item.E_OU_L8_CODE;
                dr["E_OU_L9_CODE"] = item.E_OU_L9_CODE;
                dr["E_OU_L10_CODE"] = item.E_OU_L10_CODE;
                dr["E_OU_L11_CODE"] = item.E_OU_L11_CODE;
                dr["E_OU_L12_CODE"] = item.E_OU_L12_CODE;

                var lstOrgMoreInforContractByOrgID = lstOrgMoreInforContract.Where(s => s.OrgStructureID == item.ID).FirstOrDefault();
                if (lstOrgMoreInforContractByOrgID != null)
                {
                    dr["ContactName"] = lstOrgMoreInforContractByOrgID.ContactName;
                    dr["Area"] = lstOrgMoreInforContractByOrgID.Area;
                    dr["Position"] = lstOrgMoreInforContractByOrgID.Position;
                    dr["TelephoneInforContract"] = lstOrgMoreInforContractByOrgID.Telephone;
                    dr["CellPhoneInforContract"] = lstOrgMoreInforContractByOrgID.CellPhone;
                    dr["EmailInforContract"] = lstOrgMoreInforContractByOrgID.Email;
                    dr["DescriptionInforContract"] = lstOrgMoreInforContractByOrgID.Description;
                }
                table.Rows.Add(dr);
            }


            #region Xuat excell
            if (model.ExportID != Guid.Empty)
            {
                var fullPath = ExportService.Export(model.ExportID, table, null, UserGuidID, model.ExportType);
                return Json(fullPath);
            }
            #endregion

            var _datasource = result.ToDataSourceResult(request);
            int total = result.FirstOrDefault().GetPropertyValue("TotalRow") != null ? (int)result.FirstOrDefault().GetPropertyValue("TotalRow") : 0;
            _datasource.Total = total;
            _datasource.Data = result;
            return Json(_datasource, JsonRequestBehavior.AllowGet);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho OrgStructure(OrgStructure) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllOrgStructureList([DataSourceRequest] DataSourceRequest request, CatOrgStructureSearchModel model)
        {
            return ExportAllAndReturn<Cat_OrgStructureEntity, CatOrgStructureModel, CatOrgStructureSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AllOrgStructure);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn OrgStructure (OrgStructure) theo điều kiện tìm kiếm
        public ActionResult ExportOrgStructureSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_OrgStructureEntity, CatOrgStructureModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }

        public JsonResult GetMultiOrgStructure(string text)
        {
            return GetDataForControl<CatOrgStructureMultiModel, Cat_OrgStructureMultiEntity>(text, ConstantSql.hrm_cat_sp_get_OrgStructure_multi);

        }


        public JsonResult GetMultiOrgStructureByOrgType(string text)
        {
            return GetDataForControl<CatOrgStructureMultiModel, Cat_OrgStructureMultiEntity>(text, ConstantSql.hrm_cat_sp_get_OrgStructureByOrgType_multi);

        }

        public JsonResult GetMultiOrgStructure_Cascading(string text)
        {
            //return GetDataForControl<CatOrgStructureMultiModel, Cat_OrgStructureMultiEntity>(text, ConstantSql.hrm_cat_sp_get_OrgStructure_multi);
            var arrText = text.Split('|').ToList();

            List<object> lstParam = new List<object>();
            lstParam.AddRange(new object[3]);
            lstParam[0] = (arrText[0] != null && arrText[0] != string.Empty) ? arrText[0] : null;
            lstParam[1] = (arrText[1] != null && arrText[1] != string.Empty) ? arrText[1] : null;
            lstParam[2] = (arrText[2] != null && arrText[2] != string.Empty) ? arrText[2] : null;


            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_OrgStructureMultiEntity>(lstParam, ConstantSql.hrm_cat_sp_get_OrgStructure_Cascading, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatOrgStructureMultiModel> listModel = listEntity.Translate<CatOrgStructureMultiModel>();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMultiDayOff_Cascading(string text)
        {
            var arrText = text.Split('|').ToList();

            List<object> lstParam = new List<object>();
            lstParam.AddRange(new object[3]);
            lstParam[0] = (arrText[0] != null && arrText[0] != string.Empty) ? arrText[0] : null;
            lstParam[1] = (arrText[1] != null && arrText[1] != string.Empty) ? arrText[1] : null;
            lstParam[2] = (arrText[2] != null && arrText[2] != string.Empty) ? arrText[2] : null;


            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_DayOffMultiEntity>(lstParam, ConstantSql.hrm_cat_sp_get_DayOff_Cascading, UserLogin, ref status);
            if (listEntity != null)
            {
                List<Cat_DayOffMultiModel> listModel = listEntity.Translate<Cat_DayOffMultiModel>();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }

        #region Cat_ShopGroup
        [HttpPost]
        public ActionResult GetShopGroupList([DataSourceRequest] DataSourceRequest request, Cat_ShopGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_ShopGroupModel, Cat_ShopGroupEntity, Cat_ShopGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ShopGroup);
        }
        public JsonResult GetMultiShopGroup(string text)
        {
            return GetDataForControl<Cat_ShopGroupMultiModel, Cat_ShopGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ShopGroup_multi);
        }

        public ActionResult ExportAllCat_ShopGroupList([DataSourceRequest] DataSourceRequest request, Cat_ShopGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_ShopGroupEntity, Cat_ShopGroupModel, Cat_ShopGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ShopGroup);
        }
        public ActionResult ExportCat_ShopGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ShopGroupEntity, Cat_ShopGroupModel>(selectedIds, valueFields, ConstantSql.hrm_Cat_sp_get_ShopGroupIds);
        }
        #endregion


        public ActionResult CheckExistProfileInOrgStructure(List<Guid> selectedIds)
        {
            string status = string.Empty;
            string message = string.Empty;
            var service = new Cat_OrgStructureServices();
            var listModel = new List<CatOrgStructureModel>();
            //var listEntity = service.GetDataNotParam<Cat_OrgStructureTreeViewEntity>(ConstantSql.hrm_cat_sp_get_OrgStructure_Data_SumProfile, UserLogin, ref status);
            List<Object> listObject = new List<object>();
            listObject.Add(UserLogin != null ? UserLogin : "");
            var listEntity = service.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_cat_sp_get_OrgStructure_Data_SumProfile, UserLogin, ref status);
            listModel = listEntity.Translate<CatOrgStructureModel>();
            for (int i = 0; i < selectedIds.Count; i++)
            {
                if (GetCountProfile(listModel, selectedIds[i], new int[2])[0] <= 0)
                {
                    //delete
                    message += service.Remove<Cat_OrgStructureEntity>(selectedIds[i]) + ",";

                }
                else
                {
                    message += listModel.Single(m => m.ID == selectedIds[i]).OrgStructureName + ",";

                }
            }
            message = message.Substring(0, message.LastIndexOf(','));

            return Json(message, JsonRequestBehavior.AllowGet);
            //var hrService = new Hre_ProfileServices();
            //var lstObj = new List<object>();
            //lstObj.Add(selectedIds);
            //lstObj.Add(null);
            //lstObj.Add(null);
            //var lstProfile = hrService.GetData<Hre_ProfileModel>(lstObj, ConstantSql.hrm_hr_sp_get_ProfileIdsByOrg, ref status);

            //if(lstProfile.Count == 0 || lstProfile == null){
            //    rs = true;
            //}





        }

        /// <summary>
        /// [Chuc.Nguyen] - Lấy danh sách phòng ban
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetOrgStructureTree(Guid? id, string UserName)
        {
            var service = new Cat_OrgStructureServices();
            string status = string.Empty;
            var listModel = new List<CatOrgStructureModel>();

            UserName = UserName == null ? UserLogin : UserName;

            if (HttpContext.Cache[UserName] == null)
            {
                List<Object> listObject = new List<object>();
                listObject.Add(UserName != null ? UserName : "");
                listObject.Add(null);
                var listEntity = service.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_cat_sp_get_OrgStructure_Data, UserLogin, ref status);

                #region Xử lý phân quyền cho cây phòng ban
                if (UserName == string.Empty || UserName == Common.UserNameSystem)
                {
                    listEntity.ForEach(m => m.IsShow = true);
                }
                #endregion

                if (listEntity != null)
                {
                    listModel = listEntity.Translate<CatOrgStructureModel>();

                    //var ListOrgOrderNumber = listModel.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                    //var ListOrgCode = listModel.Where(m => m.OrderOrg == null).OrderBy(m => m.Code).ToList();
                    //ListOrgOrderNumber.AddRange(ListOrgCode);
                    //listModel = ListOrgOrderNumber;

                    HttpContext.Cache[UserName] = listModel;
                }
            }
            else
            {
                listModel = HttpContext.Cache[UserName] as List<CatOrgStructureModel>;
            }

            //lấy quyền phòng ban theo user
            var orgStructure = from e in listModel
                               where (id.HasValue ? e.ParentID == id : e.ParentID == null)
                               select new
                               {
                                   id = e.ID,
                                   Name = e.Code + " - " + e.OrgStructureName,
                                   NameOrder = e.OrgStructureName,
                                   hasChildren = listModel.Any(ch => ch.ParentID == e.ID),
                                   IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + (e.Icon ?? "icon1.png"),
                                   OrderNumber = e.OrderNumber,
                                   Code = e.Code,
                                   IsShow = e.IsShow,
                                   OrderOrg = e.OrderOrg,
                                   Inactive = e.Status == "E_INACTIVE" ? true : false
                               };

            var ListOrgOrderNumber = orgStructure.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();

            var ListOrgCode = orgStructure.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
            ListOrgOrderNumber.AddRange(ListOrgCode);

            return Json(ListOrgOrderNumber, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Convert List CatOrgStructureModel To List Cat_OrgStructureForOrgTreeView
        /// </summary>
        /// <param name="listOrgstructure"></param>
        /// <returns></returns>
        public List<Cat_OrgStructureForOrgTreeView> ConvertToOrgStructureTreeView(List<CatOrgStructureModel> listOrgstructure, bool history = false)
        {
            return listOrgstructure.Select(e => new Cat_OrgStructureForOrgTreeView
            {
                id = !history ? e.ID : (e.OrgStructureID.HasValue ? e.OrgStructureID.Value : Guid.Empty),
                Name = e.Code + " - " + e.OrgStructureName,
                FullName = e.Code + " - " + e.OrgStructureName
                    + " (" + e.ProfileIsWorking
                    + "/" + (GetCountProfile(
                                listOrgstructure
                                , !history ? e.ID : (e.OrgStructureID.HasValue ? e.OrgStructureID.Value : Guid.Empty)
                                , new int[2])[0] + e.ProfileIsWorking
                            ).ToString()
                    + ")",
                NameOrder = e.OrgStructureName,
                hasChildren = false, //listOrgstructure.Any(ch => ch.ParentID == e.ID),
                IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + (e.Icon ?? "icon1.png"),
                OrderNumber = e.OrderNumber,
                Code = e.Code,
                IsShow = e.IsShow,
                IsDisable = e.IsShow,
                OrderOrg = e.OrderOrg,
                Inactive = e.Status == "E_INACTIVE" ? true : false,
                ParentID = e.ParentID,
                ProfileCount = e.ProfileCount,
                ProfileIsWorking = e.ProfileIsWorking,
                OrgColor = e.OrgColor
            })
            .ToList();
        }

        /// <summary>
        /// Generate OrgStructure Tree View
        /// </summary>
        /// <param name="listOrgStructureTreeView"></param>
        /// <param name="orgstructureParent"></param>
        public void GenerateOrgStructureTreeView(List<Cat_OrgStructureForOrgTreeView> listOrgStructureTreeView, Cat_OrgStructureForOrgTreeView orgstructureParent)
        {
            //List Child
            var listOrgStructureTreeViewChild = listOrgStructureTreeView.Where(p => p.ParentID == orgstructureParent.id);
            if (!listOrgStructureTreeViewChild.Any())
                return;

            //Create List Child
            orgstructureParent.ListChild = new List<Cat_OrgStructureForOrgTreeView>();
            orgstructureParent.hasChildren = true;

            foreach (var node in listOrgStructureTreeViewChild)
            {
                //Add Child
                orgstructureParent.ListChild.Add(node);

                //Recursive find Child in Child
                GenerateOrgStructureTreeView(listOrgStructureTreeView, node);
            }

            //Order By
            if (orgstructureParent.ListChild.Any(s => s.OrderOrg != null))
            {
                orgstructureParent.ListChild = orgstructureParent.ListChild.OrderBy(p => p.OrderOrg).ToList();
            }
            else
            {
                orgstructureParent.ListChild = orgstructureParent.ListChild.OrderBy(p => p.NameOrder).ToList();
            }

            //Set Permission parent node
            if (orgstructureParent.IsShow == false || !orgstructureParent.IsShow.HasValue)
            {
                if (orgstructureParent.ListChild.Any(p => p.IsShow == true))
                {
                    orgstructureParent.Inactive = true;
                    orgstructureParent.IsShow = true;
                }
            }
        }

        public List<Cat_OrgStructureForOrgTreeView> GenerateOrgStructure(List<Cat_OrgStructureForOrgTreeView> listOrgStructureTreeView)
        {
            var listOrgStructureTreeViewRoot = listOrgStructureTreeView
                .Where(p => p.ParentID == null)
                .OrderBy(p => p.NameOrder)
                .ToList();

            foreach (var rootNode in listOrgStructureTreeViewRoot)
            {
                GenerateOrgStructureTreeView(listOrgStructureTreeView, rootNode);
            }

            //remove root not permission
            listOrgStructureTreeViewRoot.RemoveRange(listOrgStructureTreeViewRoot.Where(p => !p.IsShow.HasValue || !p.IsShow.Value).ToList());

            return listOrgStructureTreeViewRoot.ToList();
        }

        List<Cat_OrgStructureForOrgTreeView> _ListOrgstructureTreeView = new List<Cat_OrgStructureForOrgTreeView>();
        /// <summary>
        /// HienNguyen
        /// Hàm lất dữ liệu phòng ban
        /// </summary>
        /// <param name="_listOrgstructure"></param>
        /// <param name="_currentOrg"></param>
        public void GeneralOrgStructure(List<CatOrgStructureModel> _listOrgstructure, Cat_OrgStructureForOrgTreeView _currentOrg)
        {
            //Lấy các node root
            if (!_currentOrg.HasValue())
            {
                List<Cat_OrgStructureForOrgTreeView> listOrgChild = (from e in _listOrgstructure
                                                                     where (e.ParentID == null)
                                                                     select new
                                                                     {
                                                                         id = e.ID,
                                                                         Name = e.Code + " - " + e.OrgStructureName,
                                                                         FullName = e.Code + " - " + e.OrgStructureName + " (" + e.ProfileIsWorking + "/" + (GetCountProfile(_listOrgstructure, e.ID, new int[2])[0] + e.ProfileIsWorking).ToString() + ")",
                                                                         NameOrder = e.OrgStructureName,
                                                                         hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                                         IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + (e.Icon ?? "icon1.png"),
                                                                         OrderNumber = e.OrderNumber,
                                                                         Code = e.Code,
                                                                         IsShow = e.IsShow,
                                                                         IsDisable = e.IsShow,
                                                                         OrderOrg = e.OrderOrg,
                                                                         Inactive = e.Status == "E_INACTIVE" ? true : false,
                                                                         ParentID = e.ParentID,
                                                                         ProfileCount = e.ProfileCount,
                                                                         ProfileIsWorking = e.ProfileIsWorking,
                                                                         OrgColor = e.OrgColor,
                                                                     }).OrderBy(m => m.NameOrder).ToList().Translate<Cat_OrgStructureForOrgTreeView>();

                //Code Order By
                var ListOrgOrderNumber = listOrgChild.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                var ListOrgCode = listOrgChild.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
                ListOrgOrderNumber.AddRange(ListOrgCode.ToList());

                _ListOrgstructureTreeView.AddRange(ListOrgOrderNumber.OrderBy(m => m.OrderOrg));

                foreach (var i in _ListOrgstructureTreeView)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralOrgStructure(_listOrgstructure, i);
                    }
                }
            }
            else//Không phải các node root
            {
                if (_currentOrg.ListChild == null)
                {
                    _currentOrg.ListChild = new List<Cat_OrgStructureForOrgTreeView>();
                }

                _currentOrg.ListChild.AddRange((from e in _listOrgstructure
                                                where (e.ParentID == _currentOrg.id)
                                                select new
                                                {
                                                    id = e.ID,
                                                    Name = e.Code + " - " + e.OrgStructureName,
                                                    FullName = e.Code + " - " + e.OrgStructureName + " (" + e.ProfileIsWorking + "/" + (GetCountProfile(_listOrgstructure, e.ID, new int[2])[0] + e.ProfileIsWorking).ToString() + ")",
                                                    NameOrder = e.OrgStructureName,
                                                    hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                    IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + (e.Icon ?? "icon1.png"),
                                                    OrderNumber = e.OrderNumber,
                                                    Code = e.Code,
                                                    IsShow = e.IsShow,
                                                    IsDisable = e.IsShow,
                                                    OrderOrg = e.OrderOrg,
                                                    Inactive = e.Status == "E_INACTIVE" ? true : false,
                                                    ParentID = e.ParentID,
                                                    ProfileCount = e.ProfileCount,
                                                    ProfileIsWorking = e.ProfileIsWorking,
                                                    OrgColor = e.OrgColor,
                                                }).OrderBy(m => m.NameOrder).ToList().Translate<Cat_OrgStructureForOrgTreeView>());

                //Code Order By
                var ListOrgOrderNumber = _currentOrg.ListChild.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                var ListOrgCode = _currentOrg.ListChild.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
                ListOrgOrderNumber.AddRange(ListOrgCode);
                //Son.Vo - 20170509 - hiển thị order theo tên phòng ban
                //_currentOrg.ListChild = ListOrgOrderNumber.OrderBy(s => s.NameOrder).ToList();

                //If Child Note have field IsShow is true then set all parent note field IsShow is true
                if (ListOrgOrderNumber.Any(m => m.IsShow == true))
                {
                    UpdatePermissionParentNote(_currentOrg.id);
                }

                foreach (var i in _currentOrg.ListChild)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralOrgStructure(_listOrgstructure, i);
                    }
                }
            }
        }

        /// <summary>
        /// HienNguyen
        /// Hàm lất dữ liệu phòng ban
        /// </summary>
        /// <param name="_listOrgstructure"></param>
        /// <param name="_currentOrg"></param>
        public void GeneralFolderTree(List<Cat_FolderModel> _listOrgstructure, Cat_OrgStructureForOrgTreeView _currentOrg)
        {
            //Lấy các node root
            if (!_currentOrg.HasValue())
            {
                List<Cat_OrgStructureForOrgTreeView> listOrgChild = (from e in _listOrgstructure
                                                                     where (e.ParentID == null)
                                                                     select new
                                                                     {
                                                                         id = e.ID,
                                                                         Name = e.Code + " - " + e.FolderName,
                                                                         NameOrder = e.FolderName,
                                                                         hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                                         IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                                                         OrderNumber = e.OrderNumber,
                                                                         Code = e.Code,
                                                                         IsShow = true,
                                                                         IsDisable = false,
                                                                         OrderOrg = 1,
                                                                         Inactive = false

                                                                     }).OrderBy(m => m.NameOrder).ToList().Translate<Cat_OrgStructureForOrgTreeView>();

                //Code Order By
                var ListOrgOrderNumber = listOrgChild.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                var ListOrgCode = listOrgChild.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
                ListOrgOrderNumber.AddRange(ListOrgCode.ToList());

                foreach (var item in ListOrgOrderNumber)
                {
                    item.IsShow = true;
                    item.IsDisable = true;
                }

                _ListOrgstructureTreeView.AddRange(ListOrgOrderNumber.OrderBy(m => m.OrderOrg));

                foreach (var i in _ListOrgstructureTreeView)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralFolderTree(_listOrgstructure, i);
                    }
                }
            }
            else//Không phải các node root
            {
                if (_currentOrg.ListChild == null)
                {
                    _currentOrg.ListChild = new List<Cat_OrgStructureForOrgTreeView>();
                }

                _currentOrg.ListChild.AddRange((from e in _listOrgstructure
                                                where (e.ParentID == _currentOrg.id)
                                                select new
                                                {
                                                    id = e.ID,
                                                    Name = e.Code + " - " + e.FolderName,
                                                    NameOrder = e.FolderName,
                                                    hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                    IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                                    OrderNumber = e.OrderNumber,
                                                    Code = e.Code,
                                                    IsShow = true,
                                                    IsDisable = false,
                                                    OrderOrg = 1,
                                                    Inactive = false
                                                }).OrderBy(m => m.NameOrder).ToList().Translate<Cat_OrgStructureForOrgTreeView>());

                //Code Order By
                var ListOrgOrderNumber = _currentOrg.ListChild.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                var ListOrgCode = _currentOrg.ListChild.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
                ListOrgOrderNumber.AddRange(ListOrgCode);

                foreach (var item in ListOrgOrderNumber)
                {
                    item.IsShow = true;
                    item.IsDisable = true;
                }

                _currentOrg.ListChild = ListOrgOrderNumber.ToList();

                ////If Child Note have field IsShow is true then set all parent note field IsShow is true
                //if (ListOrgOrderNumber.Any(m => m.IsShow == true))
                //{
                //    UpdatePermissionParentNote(_currentOrg.id);
                //}

                foreach (var i in _currentOrg.ListChild)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralFolderTree(_listOrgstructure, i);
                    }
                }
            }
        }


        List<Cat_CompetenceGroupModelView> _ListCompetenceGroupTreeView = new List<Cat_CompetenceGroupModelView>();

        public void GeneralCompetenceGroupTree(List<Cat_CompetenceGroupModel> _listOrgstructure, Cat_CompetenceGroupModelView _currentOrg)
        {
            //Lấy các node root
            if (!_currentOrg.HasValue())
            {
                List<Cat_CompetenceGroupModelView> listOrgChild = (from e in _listOrgstructure
                                                                   where (e.ParentID == null)
                                                                   select new
                                                                   {
                                                                       id = e.ID,
                                                                       Name = e.Code + " - " + e.CompetenceGroupName,
                                                                       NameOrder = e.CompetenceGroupName,
                                                                       hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                                       IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                                                       Code = e.Code,
                                                                       IsShow = true,
                                                                       IsDisable = false,
                                                                       OrderOrg = 1,
                                                                       Inactive = false

                                                                   }).OrderBy(m => m.NameOrder).ToList().Translate<Cat_CompetenceGroupModelView>();

                var ListOrgOrderNumber = listOrgChild.OrderBy(m => m.NameOrder).ToList();

                foreach (var item in ListOrgOrderNumber)
                {
                    item.IsShow = true;
                    item.IsDisable = true;
                }

                _ListCompetenceGroupTreeView.AddRange(ListOrgOrderNumber);

                foreach (var i in _ListCompetenceGroupTreeView)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralCompetenceGroupTree(_listOrgstructure, i);
                    }
                }
            }
            else//Không phải các node root
            {
                if (_currentOrg.ListChild == null)
                {
                    _currentOrg.ListChild = new List<Cat_CompetenceGroupModelView>();
                }

                _currentOrg.ListChild.AddRange((from e in _listOrgstructure
                                                where (e.ParentID == _currentOrg.id)
                                                select new
                                                {
                                                    id = e.ID,
                                                    Name = e.Code + " - " + e.CompetenceGroupName,
                                                    NameOrder = e.CompetenceGroupName,
                                                    hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                    IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                                    Code = e.Code,
                                                    IsShow = true,
                                                    IsDisable = false,
                                                    OrderOrg = 1,
                                                    Inactive = false
                                                }).OrderBy(m => m.NameOrder).ToList().Translate<Cat_CompetenceGroupModelView>());

                //Code Order By
                var ListOrgOrderNumber = _currentOrg.ListChild.Where(m => m.NameOrder != null).OrderBy(m => m.NameOrder).ToList();


                foreach (var item in ListOrgOrderNumber)
                {
                    item.IsShow = true;
                    item.IsDisable = true;
                }

                _currentOrg.ListChild = ListOrgOrderNumber.ToList();



                foreach (var i in _currentOrg.ListChild)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralCompetenceGroupTree(_listOrgstructure, i);
                    }
                }
            }
        }



        /// <summary>
        /// HienNguyen
        /// Hàm update lại quyền của note cha khi note con có quyền
        /// </summary>
        public void UpdatePermissionParentNote(Guid _currentOrgID)
        {
            Cat_OrgStructureForOrgTreeView notebyID = null;
            Cat_OrgStructureForOrgTreeView notebyParentID = null;

            #region Lấy dữ liệu Note By ID
            List<Cat_OrgStructureForOrgTreeView> _tmp = _ListOrgstructureTreeView;
            while (true)
            {
                if (_tmp.Count <= 0)
                {
                    break;
                }
                notebyID = _tmp.FirstOrDefault(m => m.id == _currentOrgID);

                if (notebyID != null)
                {
                    break;
                }

                _tmp = _tmp.SelectMany(m => m.ListChild).ToList();
            }
            #endregion

            if (notebyID.HasValue())
            {
                notebyID.IsShow = true;

                //Return if Parentid is null
                if (notebyID.ParentID == null)
                {
                    return;
                }

                while (true)
                {
                    #region Lấy dữ liệu Note By ID Parent
                    _tmp = _ListOrgstructureTreeView;
                    while (true)
                    {
                        if (_tmp.Count <= 0)
                        {
                            break;
                        }

                        Cat_OrgStructureForOrgTreeView tmpNoteByParentID = null;

                        if (notebyParentID != null)
                        {
                            tmpNoteByParentID = _tmp.FirstOrDefault(m => m.id == notebyParentID.ParentID);
                        }
                        else
                        {
                            tmpNoteByParentID = _tmp.FirstOrDefault(m => m.id == notebyID.ParentID);
                        }


                        if (tmpNoteByParentID != null)
                        {
                            notebyParentID = tmpNoteByParentID;
                            break;
                        }

                        _tmp = _tmp.SelectMany(m => m.ListChild).ToList();
                    }
                    #endregion

                    //notebyParentID = listAllNote.FirstOrDefault(m => m.id == notebyID.ParentID);
                    if (notebyParentID.HasValue())
                    {
                        notebyParentID.IsShow = true;

                        if (notebyParentID.ParentID == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }



        /// <summary>
        /// HienNguyen
        /// Hàm lất dữ liệu phòng ban từ bảng Cat_UnitStructure
        /// </summary>
        /// <param name="_listOrgstructure"></param>
        /// <param name="_currentOrg"></param>
        public void GeneralOrgUnitStructure(List<Cat_UnitStructureModel> _listOrgstructure, Cat_OrgStructureForOrgTreeView _currentOrg)
        {
            //Lấy các node root
            if (!_currentOrg.HasValue())
            {
                List<Cat_OrgStructureForOrgTreeView> listOrgChild = (from e in _listOrgstructure
                                                                     where (e.ParentID == null)
                                                                     select new
                                                                     {
                                                                         id = e.ID,
                                                                         Name = e.UnitCode + " - " + e.UnitName,
                                                                         NameOrder = e.UnitName,
                                                                         hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                                         IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                                                         OrderNumber = e.OrderNumber,
                                                                         Code = e.UnitCode,
                                                                         IsShow = e.IsShow,
                                                                         IsDisable = e.IsShow,
                                                                         OrderOrg = 1,
                                                                         Inactive = false
                                                                     }).ToList().Translate<Cat_OrgStructureForOrgTreeView>();

                //Code Order By
                var ListOrgOrderNumber = listOrgChild.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                var ListOrgCode = listOrgChild.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
                ListOrgOrderNumber.AddRange(ListOrgCode);

                _ListOrgstructureTreeView.AddRange(ListOrgOrderNumber);

                foreach (var i in _ListOrgstructureTreeView)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralOrgUnitStructure(_listOrgstructure, i);
                    }
                }
            }
            else//Không phải các node root
            {
                if (_currentOrg.ListChild == null)
                {
                    _currentOrg.ListChild = new List<Cat_OrgStructureForOrgTreeView>();
                }

                _currentOrg.ListChild.AddRange((from e in _listOrgstructure
                                                where (e.ParentID == _currentOrg.id)
                                                select new
                                                {
                                                    id = e.ID,
                                                    Name = e.UnitCode + " - " + e.UnitName,
                                                    NameOrder = e.UnitName,
                                                    hasChildren = _listOrgstructure.Any(ch => ch.ParentID == e.ID),
                                                    IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                                    OrderNumber = e.OrderNumber,
                                                    Code = e.UnitCode,
                                                    IsShow = e.IsShow,
                                                    IsDisable = e.IsShow,
                                                    OrderOrg = 1,
                                                    Inactive = false
                                                }).ToList().Translate<Cat_OrgStructureForOrgTreeView>());

                //Code Order By
                var ListOrgOrderNumber = _currentOrg.ListChild.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();
                var ListOrgCode = _currentOrg.ListChild.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
                ListOrgOrderNumber.AddRange(ListOrgCode);
                _currentOrg.ListChild = ListOrgOrderNumber;

                foreach (var i in _currentOrg.ListChild)
                {
                    //Nếu có tồn tại con
                    if (_listOrgstructure.Any(m => m.ParentID == i.id))
                    {
                        GeneralOrgUnitStructure(_listOrgstructure, i);
                    }
                }
            }
        }

        /// <summary>
        /// [Hien.Nguyen] - Lấy danh sách phòng ban
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrgStructureTreeNew()
        {
            var listOrgStructureTreeViewResult = HttpContext.Cache[CacheName.OrgStreeView.ToString() + UserLogin];
            if (listOrgStructureTreeViewResult == null)
            {
                try
                {
                    var service = new Cat_OrgStructureServices();
                    string status = string.Empty;

                    var listOrgStructureTreeView = CacheUtilityService.Get<List<Cat_OrgStructureForOrgTreeView>>("OrgStructureTreeView");
                    if (listOrgStructureTreeView == null)
                    {
                        var listEntity = service.GetData<CatOrgStructureModel>(new List<object> { null }, ConstantSql.hrm_cat_sp_get_OrgStructure_Data_All, UserLogin, ref status);// service.GetData<CatOrgStructureModel>(listObject, "hrm_cat_sp_get_OrgStructure_Data_VuLe", UserLogin, ref status);

                        //Convert To OrgStructure Tree
                        listOrgStructureTreeView = ConvertToOrgStructureTreeView(listEntity);
                        CacheUtilityService.AddOrUpdate("OrgStructureTreeView", listOrgStructureTreeView);
                    }

                    //Clone without reference
                    listOrgStructureTreeView = listOrgStructureTreeView.Translate<Cat_OrgStructureForOrgTreeView>();

                    #region Xử lý phân quyền cho cây phòng ban

                    if (UserLogin == string.Empty || UserLogin == Common.UserNameSystem)
                    {
                        foreach (var i in listOrgStructureTreeView)
                        {
                            i.IsShow = true;
                            i.IsDisable = true;
                        }
                    }
                    else
                    {
                        //Get With Permission
                        var listPermissionOrgStructure = service.GetDataTable(new List<object> { UserLogin != null ? UserLogin : "" },
                                ConstantSql.hrm_cat_sp_get_OrgStructure_Permission,
                                UserLogin,
                                ref status);

                        var listPermissionOrgStructureId = listPermissionOrgStructure
                            .AsEnumerable()
                            .Select(p =>
                            {
                                return (Guid)p["OrgId"];
                            }).ToList();

                        if (listPermissionOrgStructureId != null && listPermissionOrgStructureId.Any())
                        {
                            var listOrgStructureCheckPermission = listOrgStructureTreeView.Where(p => listPermissionOrgStructureId.Contains(p.id));
                            foreach (var OrgStructure in listOrgStructureCheckPermission)
                            {
                                OrgStructure.Inactive = false;
                                OrgStructure.IsShow = true;
                                OrgStructure.IsDisable = true;
                            }
                        }
                    }

                    #endregion

                    listOrgStructureTreeViewResult = GenerateOrgStructure(listOrgStructureTreeView);
                    HttpContext.Cache[CacheName.OrgStreeView.ToString() + UserLogin] = listOrgStructureTreeViewResult;
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }

            var jsonResult = Json(listOrgStructureTreeViewResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        /// <summary>
        /// [Hien.Nguyen] - Lấy danh sách phòng ban Unit
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrgUnitStructureTreeNew()
        {

            if (HttpContext.Cache[CacheName.OrgUnitTreeView.ToString() + UserLogin] == null)
            {
                var service = new Cat_OrgStructureServices();
                string status = string.Empty;
                List<Object> listObject = Common.AddRange(5);
                var listEntity = service.GetData<Cat_UnitStructureModel>(listObject, ConstantSql.hrm_cat_sp_get_UnitStructure, UserLogin, ref status);
                listEntity.ForEach(m => m.IsShow = true);

                _ListOrgstructureTreeView = new List<Cat_OrgStructureForOrgTreeView>();
                GeneralOrgUnitStructure(listEntity, null);

                HttpContext.Cache[CacheName.OrgUnitTreeView.ToString() + UserLogin] = _ListOrgstructureTreeView;
            }
            else
            {
                _ListOrgstructureTreeView = HttpContext.Cache[CacheName.OrgUnitTreeView.ToString() + UserLogin] as List<Cat_OrgStructureForOrgTreeView>;
            }

            return Json(_ListOrgstructureTreeView, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Hien.Nguyen] - Lấy danh sách phòng ban Unit
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFolderTree()
        {
            if (HttpContext.Cache[CacheName.FolderTree.ToString() + UserLogin] == null)
            {
                var service = new Cat_OrgStructureServices();
                string status = string.Empty;
                List<Object> listObject = new List<object>();
                listObject.Add(null);
                listObject.Add(null);
                var listEntity = service.GetDataNotParam<Cat_FolderModel>(ConstantSql.hrm_cat_sp_get_Folder_Data, UserLogin, ref status);

                _ListOrgstructureTreeView = new List<Cat_OrgStructureForOrgTreeView>();
                GeneralFolderTree(listEntity, null);

                HttpContext.Cache[CacheName.FolderTree.ToString() + UserLogin] = _ListOrgstructureTreeView;
            }
            else
            {
                _ListOrgstructureTreeView = HttpContext.Cache[CacheName.FolderTree.ToString() + UserLogin] as List<Cat_OrgStructureForOrgTreeView>;
            }

            return Json(_ListOrgstructureTreeView, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCompetenceGroupTree()
        {


            if (HttpContext.Cache[CacheName.CompetenceGroupView.ToString() + UserLogin] == null)
            {
                var service = new Cat_OrgStructureServices();
                string status = string.Empty;
                var objCompetenGroup = Common.AddRange(4);
                var listModel = service.GetData<Cat_CompetenceGroupModel>(objCompetenGroup, ConstantSql.hrm_cat_sp_get_CompetenceGroup, UserLogin, ref status);

                _ListCompetenceGroupTreeView = new List<Cat_CompetenceGroupModelView>();
                GeneralCompetenceGroupTree(listModel, null);

                HttpContext.Cache[CacheName.CompetenceGroupView.ToString() + UserLogin] = _ListCompetenceGroupTreeView;
            }
            else
            {
                _ListCompetenceGroupTreeView = HttpContext.Cache[CacheName.CompetenceGroupView.ToString() + UserLogin] as List<Cat_CompetenceGroupModelView>;
            }

            return Json(_ListCompetenceGroupTreeView, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetFolderTree(Guid? id, string UserName)
        //{
        //    var service = new BaseService();
        //    string status = string.Empty;
        //    var listModel = new List<Cat_FolderModel>();
        //    var CacheName = "FolderTree";
        //    if (HttpContext.Cache[CacheName] == null)
        //    {
        //        List<Object> listObject = new List<object>();
        //        listObject.Add(UserName != null ? UserName : "");
        //        listObject.Add(null);
        //        var listEntity = service.GetDataNotParam<Cat_FolderModel>(ConstantSql.hrm_cat_sp_get_Folder_Data, UserLogin, ref status);
        //        if (listEntity != null)
        //        {
        //            listModel = listEntity.Translate<Cat_FolderModel>();
        //            HttpContext.Cache[CacheName] = listModel;
        //        }
        //    }
        //    else
        //    {
        //        listModel = HttpContext.Cache[CacheName] as List<Cat_FolderModel>;
        //    }

        //    var orgStructure = from e in listModel
        //                       where (id.HasValue ? e.ParentID == id : e.ParentID == null)
        //                       select new
        //                       {
        //                           id = e.ID,
        //                           Name = e.FolderName,
        //                           NameOrder = e.FolderName,
        //                           hasChildren = listModel.Any(ch => ch.ParentID == e.ID),
        //                           IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + "Folder.png",
        //                           OrderNumber = e.OrderNumber,
        //                           Code = e.Code,
        //                           IsShow = true,
        //                           Inactive = false,

        //                       };
        //    return Json(orgStructure, JsonRequestBehavior.AllowGet);
        //} 



        /// <summary>
        /// [23/11/2015][hien.nguyen][Modify][59537]
        /// Hàm get dữ liệu cây phòng ban dùng có Inactive
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public JsonResult GetOrgStructureTreeForProjectPermission(Guid? id, string UserName)
        {
            var service = new Cat_OrgStructureServices();
            string status = string.Empty;
            var listModel = new List<CatOrgStructureModel>();
            List<Object> listObject = new List<object>();
            listObject.Add(UserName != null ? UserName : "");
            listObject.Add("E_INACTIVE");
            var listEntity = service.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_cat_sp_get_OrgStructure_Data, UserLogin, ref status);

            #region Xử lý phân quyền cho cây phòng ban
            if (UserName == string.Empty || UserName == Common.UserNameSystem)
            {
                listEntity.ForEach(m => m.IsShow = true);
            }
            #endregion

            if (listEntity != null)
            {
                listModel = listEntity.Translate<CatOrgStructureModel>();
            }

            //lấy quyền phòng ban theo user
            var orgStructure = from e in listModel
                               where (id.HasValue ? e.ParentID == id : e.ParentID == null)
                               select new
                               {
                                   id = e.ID,
                                   Name = e.Code + " - " + e.OrgStructureName,
                                   NameOrder = e.OrgStructureName,
                                   hasChildren = listModel.Any(ch => ch.ParentID == e.ID),
                                   IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + (e.Icon ?? "icon1.png"),
                                   OrderNumber = e.OrderNumber,
                                   Code = e.Code,
                                   IsShow = e.IsShow,
                                   OrderOrg = e.OrderOrg,
                                   Inactive = e.Status == "E_INACTIVE" ? true : false
                               };

            var ListOrgOrderNumber = orgStructure.Where(m => m.OrderOrg != null).OrderBy(m => m.OrderOrg).ToList();

            var ListOrgCode = orgStructure.Where(m => m.OrderOrg == null).OrderBy(m => m.NameOrder).ToList();
            ListOrgOrderNumber.AddRange(ListOrgCode);

            return Json(ListOrgOrderNumber, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Clear all cache
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCache()
        {
            List<string> keys = new List<string>();

            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = HttpContext.Cache.GetEnumerator();

            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            //Loại bỏ cache lưu tính lương
            keys = keys.Where(m => m != CacheName.ComputeProgress.ToString()).ToList();

            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                HttpContext.Cache.Remove(keys[i]);
            }

            return Json("");
        }


        /// <summary>
        /// Action Update Render Orgunit
        /// </summary>
        /// <returns></returns>
        /// 
        //ToLe-20150914-0056269
        public void UpdateRenderOrgunit([DataSourceRequest] DataSourceRequest request, Cat_OrgUnitModel Model)
        {
            var service = new Cat_OrgUnitServices();
            string status = string.Empty;
            service.ExecStore<Cat_OrgUnitEntity>(ConstantSql.hrm_cat_sp_update_render_orgunit, UserLogin, ref status);

        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateRenderAddress()
        {
            var service = new Cat_OrgUnitServices();
            string status = string.Empty;
            service.ExecStore<Cat_AddressEntity>(ConstantSql.hrm_cat_sp_update_render_Address, UserLogin, ref status);

        }

        /// <summary>
        /// Action clear cache treeview orgstructure
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public JsonResult ClearCacheOrgStructure()
        {
            HttpContext.Cache.Remove("List_OrgStructureTreeView");
            HttpContext.Cache.Remove("List_OrgStructureTreeViewSumProfile");
            HttpContext.Cache.Remove("List_OrgStructureTreeViewbyHistorySumProfile");
            HttpContext.Cache.Remove("List_OrgStructureTreeViewbyHistorySumProfile_" + UserLogin);
            HttpContext.Cache.Remove("List_CompetenceGroupTreeView");
            HttpContext.Cache.Remove("List_CompetenceGroupTreeView_" + UserLogin);
            return Json("");
        }

        /// <summary>
        /// [HienNguyen]
        /// Hàm show lại các node parent khi node con có quyền mà parent ko có quyền
        /// </summary>
        /// <param name="listOrg"></param>
        /// <returns></returns>
        public List<CatOrgStructureModel> ShowParentOrg(List<CatOrgStructureModel> listOrg)
        {
            var listOrgHiden = listOrg.Where(m => m.IsShow == true).ToList();
            foreach (var org in listOrgHiden)
            {
                var tmp = org;
                while (true)
                {
                    var orgItem = listOrg.FirstOrDefault(m => m.ID == tmp.ParentID);
                    if (!orgItem.HasValue())
                    {
                        break;
                    }
                    if (orgItem.IsShow == false)
                    {
                        orgItem.Status = "E_INACTIVE";
                    }
                    tmp = orgItem;
                }
            }
            return listOrg;
        }

        /// <summary>
        /// [HienNguyen]
        /// Hàm show lại các node parent khi node con có quyền mà parent ko có quyền
        /// </summary>
        /// <param name="listOrg"></param>
        /// <returns></returns>
        public List<Cat_OrgStructureTreeViewEntity> ShowParentOrg(List<Cat_OrgStructureTreeViewEntity> listOrg)
        {
            var listOrgHiden = listOrg.Where(m => m.IsShow == true).ToList();
            foreach (var org in listOrgHiden)
            {
                var tmp = org;
                while (true)
                {
                    var orgItem = listOrg.FirstOrDefault(m => m.ID == tmp.ParentID);
                    if (!orgItem.HasValue())
                    {
                        break;
                    }
                    if (orgItem.IsShow == false)
                    {
                        orgItem.Status = "E_INACTIVE";
                    }
                    tmp = orgItem;
                }
            }
            return listOrg;
        }

        /// <summary>
        /// [Hien.Nguyen] Lấy danh sách phòng ban tổng hợp nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        //[OutputCache(NoStore = true, Duration = 0)]
        public JsonResult GetOrgStructureTreeSummaryProfile(Guid? id, string UserName)
        {
            var listOrgStructureTreeViewResult = HttpContext.Cache["List_OrgStructureTreeViewSumProfile_" + UserLogin];
            if (listOrgStructureTreeViewResult == null)
            {
                var service = new Cat_OrgStructureServices();
                string status = string.Empty;

                List<Object> listObject = new List<object>();
                listObject.Add(UserLogin != null ? UserLogin : "");
                var listTreeViewEntity = service.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_cat_sp_get_OrgStructure_Data_SumProfile, UserLogin, ref status);
                var listModel = listTreeViewEntity.Translate<CatOrgStructureModel>();

                var listOrgStructureTreeView = ConvertToOrgStructureTreeView(listModel);
                listOrgStructureTreeViewResult = GenerateOrgStructure(listOrgStructureTreeView);

                if (listOrgStructureTreeViewResult != null)
                {
                    HttpContext.Cache.Add("List_OrgStructureTreeViewSumProfile_" + UserLogin
                        , listOrgStructureTreeViewResult
                        , null, DateTime.Now.AddDays(30)
                        , TimeSpan.Zero
                        , CacheItemPriority.Default
                        , null);
                }
            }

            var json = Json(listOrgStructureTreeViewResult, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        /// <summary>
        /// [Hien.Nguyen] Hàm xử lý đếm tổng số nhân viên và nhân viên đang làm việc trong phòng ban
        /// </summary>
        /// <param name="source"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int[] GetCountProfile(List<CatOrgStructureModel> source, Guid id, int[] value)
        {
            //Get các phòng ban con
            var listChild = source.Where(m => m.ParentID == id).ToList();

            //Điều kiện dừng
            if (listChild.Count <= 0)
            {
                return new int[2];
            }
            //Duyệt qua các con và chạy đệ quy tìm child của các phòng ban con
            foreach (var i in listChild)
            {
                value[0] += i.ProfileIsWorking;
                value[1] += i.ProfileCount;
                GetCountProfile(source, i.ID, value);
            }
            return value;
        }




        #region OrgStructureParentAndChild
        //public List<OrgStructureParentAndChild> OrgStructureParentAndChild(int? id)
        //{
        //    var service = new Cat_OrgStructureServices();
        //    string status = string.Empty;
        //    var listObj = new List<object>();
        //    //add para giả không có giá trị
        //    for (int i = 0; i < 5; i++)
        //    {
        //        listObj.Add(null);
        //    }
        //    var model = new OrgStructureParentAndChild();
        //    var listmodel = new List<OrgStructureParentAndChild>();
        //    var listIdTemp = new List<int>();
        //    var listIdTemp1 = new List<int>();
        //    var tempDic = new Dictionary<int, List<int>>();
        //    var listEntity = service.GetData<Cat_OrgStructureEntity>(listObj,ConstantSql.hrm_cat_sp_get_OrgStructure, ref status);
        //    if (listEntity != null)
        //    {
        //        foreach (var item in listEntity)
        //        {
        //            listIdTemp = GetChild(item.Id, listEntity);
        //            tempDic.Add(item.Id,listIdTemp);
        //        }

        //        foreach (var item in tempDic.Where(s=>s.Value.Count >0))
        //        {
        //            GetChildsTemp(item.Key, tempDic);
        //            while (GetChildsTemp(item.Key, tempDic).Count > 0)
        //            {
        //                GetChildsTemp(item.Key, tempDic);
        //            }
        //        } 

        //        //var listChild = GetChilds(listEntity);

        //        //foreach (var item in listChild.Keys)
        //        //{
        //        //    listIdTemp.AddRange(listId);
        //        //    while (listId.Count>0)
        //        //    {

        //        //    }

        //        //    listIdTemp.AddRange(listId);

        //        //    for(int i = 0; i<listId.Count; i++)
        //        //    {
        //        //        while (listId.Count > 0)
        //        //        {
        //        //            listId = GetChild(listId[i], listEntity);

        //        //        }
        //        //        model.Childs.AddRange(listChildNode);
        //        //        model.CountChild += listChildNode.Count;
        //        //    }
        //        //    if (model.Childs.Count > 0)
        //        //        listmodel.Add(model);
        //        //}
        //    }
        //    return listmodel;
        //}

        //public List<int> GetChilds(int id, Dictionary<int, List<int>> dictionary)
        //{
        //    return dictionary.Where(w => w.Key == id).Select(s => s.Value).FirstOrDefault();
        //}

        //public List<int> GetChildsTemp(int id, Dictionary<int, List<int>> dictionary)
        //{
        //    var count = dictionary.Count();
        //    var listId = new List<int>();
        //    while (count > 0)
        //    {
        //        var dataIds = GetChilds(id, dictionary);
        //        if (dataIds != null)
        //        {
        //            listId.AddRange(dataIds);
        //            foreach (var item in dataIds)
        //            {
        //                dataIds = GetChilds(item, dictionary);
        //                listId.AddRange(dataIds);
        //            }
        //        }
        //    }



        //    return dictionary.Where(w => w.Key == id).Select(s => s.Value).FirstOrDefault();
        //}


        //public List<int> GetChild(int parent, List<Cat_OrgStructureEntity> data)
        //{
        //    var listId = new List<int>();
        //    foreach(var item in data)
        //    {
        //        if (item.ParentID ==parent)
        //        listId.Add(item.Id);
        //    }
        //    return listId;
        //} 
        #endregion


        #endregion

        #region Cat_JobTitle
        [HttpPost]
        public ActionResult GetlstJobtitleByOrgStructureID(Guid? orgStructureID)
        {
            var result = new List<Cat_JobTitleEntity>();
            if (orgStructureID != null)
            {
                var profileServices = new Hre_ProfileServices();
                result = profileServices.GetlstJobtitleByOrgStructureID(orgStructureID.Value);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng JobTitle (Cat_JobTitle)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetJobTitleList([DataSourceRequest] DataSourceRequest request, Cat_JobTitleSearchModel model)
        {
            return GetListDataAndReturn<Cat_JobTitleModel, Cat_JobTitleEntity, Cat_JobTitleSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_JobTitle);
        }
        public JsonResult GetJobTitleOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_JobTitleMultiEntity>(text, ConstantSql.hrm_cat_sp_get_JobTitle_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<Cat_JobTitleMultiModel> listModel = listEntity.Translate<Cat_JobTitleMultiModel>();
                listModel = listModel.OrderBy(s => s.JobTitleName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiJobTitle(string text)
        {
            return GetDataForControl<Cat_JobTitleMultiModel, Cat_JobTitleMultiEntity>(text, ConstantSql.hrm_cat_sp_get_JobTitle_Multi);
        }
        public JsonResult GetMultiJobTitleOrderNumber(string text)
        {
            return GetDataForControl<Cat_JobTitleMultiOrderNumberModel, Cat_JobTitleMultiOrderNumberEntity>(text, ConstantSql.hrm_cat_sp_get_JobTitle_Multi_OrderNumber);
        }

        public JsonResult GetMutiJobtitleByOrgStructureIDs(string orgStructureIDIds)
        {
            return GetDataForControl<Cat_JobTitleMultiModel, Cat_JobTitleMultiEntity>(orgStructureIDIds, ConstantSql.hrm_cat_sp_get_JobTitle_MultiByOrgStructureID);
        }

        public JsonResult GetJobTitle()
        {
            //var service = new Cat_JobTitleServices();
            var result = baseService.GetAllUseEntity<Cat_JobTitleEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_JobTitle(Cat_JobTitle) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllJobTitleList([DataSourceRequest] DataSourceRequest request, Cat_JobTitleSearchModel model)
        {
            return ExportAllAndReturn<Cat_JobTitleEntity, Cat_JobTitleModel, Cat_JobTitleSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_JobTitle);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_JobTitle (Cat_JobTitle) theo điều kiện tìm kiếm
        public ActionResult ExportJobTitleSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_JobTitleEntity, Cat_JobTitleModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        #endregion

        #region Cat_Position
        public ActionResult EditPosition(Cat_PositionSaveModel model)
        {
            #region Validate
            string message = string.Empty;
            if (model.Tab_GeneralInformation != null)
            {
                var checkValidate = HRM.Business.Main.Domain.ValidatorService.OnValidateData<Tab_GeneralInformationModel>(LanguageCode, model.Tab_GeneralInformation, "Cat_Position", ref message);
                if (!checkValidate)
                {
                    model.ActionStatus = message;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            if (model.Tab_JobDescription != null)
            {
                var checkValidate = HRM.Business.Main.Domain.ValidatorService.OnValidateData<Tab_JobDescriptionModel>(LanguageCode, model.Tab_JobDescription, "Cat_Position", ref message);
                if (!checkValidate)
                {
                    model.ActionStatus = message;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            if (model.Tab_CandidateCriteria != null)
            {
                var checkValidate = HRM.Business.Main.Domain.ValidatorService.OnValidateData<Tab_CandidateCriteriaModel>(LanguageCode, model.Tab_CandidateCriteria, "Cat_Position", ref message);
                if (!checkValidate)
                {
                    model.ActionStatus = message;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion

            var PositionSave = new CatPositionModel();
            Guid positionID = Guid.Empty;

            if (model.Tab_CandidateCriteria != null)
            {
                foreach (var item in model.Tab_CandidateCriteria.GetType().GetProperties())
                {
                    PositionSave.SetPropertyValue(item.Name, model.Tab_CandidateCriteria.GetPropertyValue(item.Name));
                }
            }
            if (model.Tab_JobDescription != null)
            {
                foreach (var item in model.Tab_JobDescription.GetType().GetProperties())
                {
                    PositionSave.SetPropertyValue(item.Name, model.Tab_JobDescription.GetPropertyValue(item.Name));
                }
            }
            if (model.Tab_GeneralInformation != null)
            {
                foreach (var item in model.Tab_GeneralInformation.GetType().GetProperties())
                {
                    PositionSave.SetPropertyValue(item.Name, model.Tab_GeneralInformation.GetPropertyValue(item.Name));
                }
            }

            var service = new ActionService(UserLogin, LanguageCode);
            var modelRef = service.UpdateOrCreate<Cat_PositionEntity, CatPositionModel>(PositionSave);
            positionID = modelRef.ID;
            model.ActionStatus = modelRef.ActionStatus;
            model.ID = positionID;
            if (model.Tab_JobDescription != null)
            {
                model.Tab_JobDescription.ID = positionID;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetlstPositionByJobtitle(Guid? orgStructureID, Guid? jobtitleID)
        {
            var hre_ProfileServices = new Hre_ProfileServices();
            if (orgStructureID != null && jobtitleID != null)
            {
                var result = hre_ProfileServices.GetlstPositionByJobtitle(orgStructureID.Value, jobtitleID.Value);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost]
        public ActionResult GetlstPositionByOrgAndJobtitle(Guid? orgStructureID, Guid? jobtitleID)
        {
            var hre_ProfileServices = new Hre_ProfileServices();
            var result = new List<Cat_PositionEntity>();
            if (orgStructureID != null)
            {
                result = hre_ProfileServices.GetlstPositionByOrgAndJobtitle(orgStructureID.Value, jobtitleID);
            }
            else
            {
                result = hre_ProfileServices.GetlstPositionByOrgAndJobtitle(Guid.Empty, jobtitleID);
            }
            //Quyen.Quach 29/01/2018 
            if (result != null)
            {
                foreach (var item in result)
                {
                    item.PositionName = item.Code + " - " + item.PositionName;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetlstPositionByOrgOrderNumber(string orgStructureID)
        {
            var result = new List<Cat_PositionEntity>();
            if (!string.IsNullOrEmpty(orgStructureID))
            {
                List<int> lstOrderNumber = orgStructureID.Split(',').Select(x => int.Parse(x)).ToList();
                var profileServices = new Hre_ProfileServices();
                result = profileServices.GetlstPositionByOrgOrderNumber(lstOrderNumber);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetlstPositionByOrgStructureID(string orgStructureID)
        {
            var result = new List<Cat_PositionEntity>();
            if (!string.IsNullOrEmpty(orgStructureID))
            {
                Guid orgID = Guid.Parse(orgStructureID);
                var profileServices = new Hre_ProfileServices();
                result = profileServices.GetlstPositionByOrgStructureID(orgID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng Position (Cat_Position)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPositionList([DataSourceRequest] DataSourceRequest request, CatPositionSearchModel model)
        {
            return GetListDataAndReturn<CatPositionModel, Cat_PositionEntity, CatPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Position);
        }

        public JsonResult GetPositionOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_PositionMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Position_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatPositionMultiModel> listModel = listEntity.Translate<CatPositionMultiModel>();
                listModel = listModel.OrderBy(s => s.PositionName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiPosition(string text)
        {
            return GetDataForControl<CatPositionMultiModel, CatPositionMultiModel>(text, ConstantSql.hrm_cat_sp_get_Position_Multi);
        }
        public JsonResult GetMultiPositionByJobTitleId(string jobTitleID)
        {
            if (jobTitleID == string.Empty)
            {
                var emptyGuid = Guid.Empty;
                return GetDataForControl<CatPositionMultiModel, CatPositionMultiModel>(emptyGuid + "", ConstantSql.hrm_cat_sp_get_Position_MultiByJobTitleId);
            }
            return GetDataForControl<CatPositionMultiModel, CatPositionMultiModel>(jobTitleID, ConstantSql.hrm_cat_sp_get_Position_MultiByJobTitleId);
        }

        public JsonResult GetMultiPositionByOrg(string text, string strOrderNumber)
        {
            string status = string.Empty;
            var services = new ActionService(LanguageCode);
            var obj = new List<object>();
            obj.AddRange(new object[5]);
            obj[0] = text;
            obj[1] = strOrderNumber;
            obj[2] = 1;
            obj[3] = int.MaxValue - 1;
            var result = services.GetData<Cat_PositionMultiEntity>(obj, ConstantSql.hrm_cat_sp_get_Position_MultiByOrg, ref status);
            if (result != null)
                return Json(result, JsonRequestBehavior.AllowGet);
            return null;
        }

        public JsonResult GetPosition()
        {
            var result = baseService.GetAllUseEntity<Cat_PositionEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_Position(Cat_Position) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllPositionList([DataSourceRequest] DataSourceRequest request, CatPositionSearchModel model)
        {
            return ExportAllAndReturn<Cat_PositionEntity, CatPositionModel, CatPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Position);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_Position (Cat_Position) theo điều kiện tìm kiếm
        public ActionResult ExportPositionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PositionEntity, CatPositionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }

        #endregion

        #region Cat_ContractType
        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng ContractType (Cat_ContractType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetContractTypeList([DataSourceRequest] DataSourceRequest request, CatContractTypeSearchModel model)
        {
            return GetListDataAndReturn<CatContractTypeModel, Cat_ContractTypeEntity, CatContractTypeSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_ContractType);
        }
        public JsonResult GetMultiModelCode(string text)
        {
            return GetDataForControl<Cat_ModelModel, Cat_ModelEntity>(text, ConstantSql.hrm_Cat_SP_GET_ModeCode_Multi);
        }
        public JsonResult GetMultiColorCode(string text)
        {
            return GetDataForControl<PUR_ColorModelModel, PUR_ColorModelEntity>(text, ConstantSql.hrm_Cat_SP_GET_ColorCode_Multi);
        }
        #region Cat_PaymentMethod
        public JsonResult GetCat_PaymentMethodList([DataSourceRequest] DataSourceRequest request, Cat_PaymentMethodModel model)
        {
            var _action = new ActionService(UserLogin);
            string status = string.Empty;
            var Listobj = new List<object>();
            Listobj.AddRange(new object[4]);
            Listobj[0] = model.PaymentMethod;
            Listobj[1] = model.DeadlinePayment;
            Listobj[2] = 1;
            Listobj[3] = int.MaxValue - 1;
            var result = _action.GetData<Cat_PaymentMethodEntity>(Listobj, ConstantSql.hrm_Cat_SP_GET_PaymentMethod, ref status);
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiPaymentMethod(string text)
        {
            return GetDataForControl<Cat_PaymentMethodModel, Cat_PaymentMethodEntity>(text, ConstantSql.hrm_Cat_SP_GET_PaymentMethod_Multi);
        }

        public JsonResult GetMultiCatGradeCfg(string text)
        {
            return GetDataForControl<Cat_GradeCfgEntity, Cat_GradeCfgEntity>(text, ConstantSql.hrm_cat_sp_get_gradecfg_multi);
        }

        public JsonResult GETPaymentMethod(Guid ID)
        {
            ActionService action = new ActionService(UserLogin);
            string status = string.Empty;
            var result = action.GetByIdUseStore<Cat_PaymentMethodEntity>(ID, ConstantSql.hrm_Cat_SP_GET_PaymentMethodByID, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GETPurPaymentByMIDandPMID(Guid ModelID, Guid PaymentID)
        {
            ActionService action = new ActionService(UserLogin);
            string status = string.Empty;
            List<object> _obj = new List<object>();
            _obj.AddRange(new object[2]);
            _obj[0] = ModelID;
            _obj[1] = PaymentID;
            var result = action.GetData<PUR_PaymentModelEntity>(_obj, ConstantSql.hrm_cat_sp_get_PurpaymentByModelId_PMID, ref status).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GETPaymentMethodByModelID(Guid ID)
        {
            string _id = ID != null ? Common.DotNetToOracle(ID.ToString()) : string.Empty;
            ActionService action = new ActionService(UserLogin);
            string status = string.Empty;
            var result = action.GetData<Cat_PaymentMethodEntity>(_id, ConstantSql.hrm_Cat_SP_GET_PAYMENTBYMODELID, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GETPaymentMethodByModelIDGrid([DataSourceRequest] DataSourceRequest request, Guid? ModelID)
        {
            var result = new List<PUR_PaymentModelEntity>();
            if (ModelID != Guid.Empty && ModelID != null)
            {
                ActionService action = new ActionService(UserLogin);
                string status = string.Empty;
                result = action.GetData<PUR_PaymentModelEntity>(Common.DotNetToOracle(ModelID.ToString()), ConstantSql.hrm_Cat_SP_GET_PURPMMBYMODELID, ref status);
            }
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetMultiReceivePlace(string text)
        {
            return GetDataForControl<Cat_ReceivePlaceModel, Cat_ReceivePlaceEntity>(text, ConstantSql.hrm_Cat_SP_GET_ReceivePlace_Multi);
        }

        public JsonResult GetMultiContractType(string text, string UserID)
        {
            return GetDataForControl<CatContractTypeMultiModel, Cat_ContractTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ContractType_multi);
        }
        public JsonResult GetMultiContractTypeMinusNullOrderNumber(string text, string UserID)
        {
            return GetDataForControl<CatContractTypeMultiModel, Cat_ContractTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ContractTypeMinusNullOrderNumber_multi);
        }

        public JsonResult GetRecivePlaceByModelType(string text)
        {
            return GetDataForControl<Cat_ReceivePlaceModel, Cat_ReceivePlaceEntity>(text, ConstantSql.hrm_cat_sp_get_recplabymodeltype);
        }

        public JsonResult GetMultiNextContractType(string text, string UserID)
        {
            var services = new ActionService(UserLogin);
            var status = string.Empty;
            var result = services.GetData<CatContractTypeMultiModel>(text, ConstantSql.hrm_cat_sp_get_ContractType_multi, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
            //Son.Vo - 20161203 - 0076421 - load all loại
            //if (result != null)
            //{
            //    result = result.Where(s => s.Type != null && s.Type != EnumDropDown.TypeContract.E_PROBATION.ToString() && s.Type != EnumDropDown.TypeContract.E_APPRENTICESHIP.ToString()).ToList();
            //}
            //else
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}
        }
        public JsonResult GetMultiAppendixContractType(string text)
        {
            return GetDataForControl<CatAppendixContractTypeMultiModel, Cat_AppendixContractTypeEntity>(text, ConstantSql.hrm_cat_sp_get_AppendixContractType_multi);
        }

        public JsonResult GetMultiAppConTypeForExtend(string text)
        {
            return GetDataForControl<CatAppendixContractTypeMultiModel, Cat_AppendixContractTypeEntity>(text, ConstantSql.hrm_cat_sp_get_AppConTypeForExtend_multi);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_ContractType(Cat_ContractType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllContractTypeList([DataSourceRequest] DataSourceRequest request, CatContractTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_ContractTypeEntity, CatContractTypeModel, CatContractTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ContractType);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_ContractType (Cat_ContractType) theo điều kiện tìm kiếm
        public ActionResult ExportContractTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ContractTypeEntity, CatContractTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        [HttpPost]
        public ActionResult GetMonthOfDateEndProbation(Guid ContractTypeID, DateTime? DateHire)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            DateTime date = DateTime.Now;
            if (DateHire.HasValue)
            {
                date = DateHire.Value;
            }
            string status = "";
            var rs = service.GetByIdUseStore<Cat_ContractTypeEntity>(ContractTypeID, ConstantSql.hrm_cat_sp_get_ContractTypeById, ref status);
            if (rs != null && rs.ValueTime.HasValue)
            {
                if (rs.UnitTime == HRM.Infrastructure.Utilities.EnumDropDown.UnitType.E_MONTH.ToString())
                {
                    date = date.AddMonths(int.Parse(rs.ValueTime.Value.ToString()));
                }
                else if (rs.UnitTime == HRM.Infrastructure.Utilities.EnumDropDown.UnitType.E_YEAR.ToString())
                {
                    date = date.AddYears(int.Parse(rs.ValueTime.Value.ToString()));
                }
                rs.DateEnd = date;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_EmployeeType
        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng EmployeeType (Cat_EmployeeType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEmployeeTypeList([DataSourceRequest] DataSourceRequest request, CatEmployeeTypeSearchModel model)
        {
            return GetListDataAndReturn<CatEmployeeTypeModel, Cat_EmployeeTypeEntity, CatEmployeeTypeSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_EmployeeType);
        }

        public JsonResult GetEmployeeTypeOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_EmployeeTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_EmployeeType_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatEmployeeTypeMultiModel> listModel = listEntity.Translate<CatEmployeeTypeMultiModel>();
                listModel = listModel.OrderBy(s => s.EmployeeTypeName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListEmploymentType()
        {
            IList<SelectListItem> listEmploymentType = Enum.GetValues(typeof(EnumDropDown.EmploymentType))
                     .Cast<EnumDropDown.EmploymentType>()
                     .Select(x => new SelectListItem { Value = x.ToString(), Text = x.ToString().TranslateString() })
                     .ToList();
            listEmploymentType = listEmploymentType.OrderBy(s => s.Text).ToList();
            return Json(listEmploymentType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListGender()
        {
            IList<SelectListItem> listGender = Enum.GetValues(typeof(EnumDropDown.Gender))
                .Cast<EnumDropDown.Gender>()
                .Select(x => new SelectListItem { Value = x.ToString(), Text = x.ToString().TranslateString() }).OrderBy(p => p.Text)
                .ToList();
            listGender = listGender.OrderBy(p => p.Text).ToList();
            return Json(listGender, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListMarriage()
        {
            IList<SelectListItem> listMarriageStatus = Enum.GetValues(typeof(EnumDropDown.MarriageStatus))
                  .Cast<EnumDropDown.MarriageStatus>()
                  .Select(x => new SelectListItem { Value = x.ToString(), Text = x.ToString().TranslateString() }).OrderBy(p => p.Text)
                  .ToList();
            listMarriageStatus = listMarriageStatus.OrderBy(s => s.Text).ToList();
            return Json(listMarriageStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiEmployeeType(string text)
        {
            return GetDataForControl<CatEmployeeTypeMultiModel, Cat_EmployeeTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_EmployeeType_Multi);
        }
        /// <summary>
        /// [Tho.Bui]:Multi theo AccidentType
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult GetMultiAccidentType(string text)
        {
            return GetDataForControl<Cat_AccidentTypeMutiModel, Cat_AccidentTypeEntity>(text, ConstantSql.hrm_cat_sp_get_AccidentType_MultiNew);
        }
        public JsonResult GetEmpType()
        {
            var result = baseService.GetAllUseEntity<Cat_EmployeeTypeEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_EmployeeType(Cat_EmployeeType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllEmployeeTypeList([DataSourceRequest] DataSourceRequest request, CatEmployeeTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_EmployeeTypeEntity, CatEmployeeTypeModel, CatEmployeeTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EmployeeType);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_EmployeeType (Cat_EmployeeType) theo điều kiện tìm kiếm
        public ActionResult ExportEmployeeTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_EmployeeTypeEntity, CatEmployeeTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }

        #endregion

        #region Cat_NameEntity

        public JsonResult GetEntityByID(Guid? ID)
        {
            if (ID != null && ID != Guid.Empty)
            {
                var _NameEntityServices = new Cat_NameEntityServices();
                var entity = _NameEntityServices.GetDataEntityByID(ID.Value);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #region Cat_TypeOfRetirement
        [HttpPost]
        public ActionResult GetTypeOfRetirementList([DataSourceRequest] DataSourceRequest request, Cat_TypeOfRetirementSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_TypeOfRetirementSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TypeOfRetirement);
        }

        public ActionResult ExportAllOfRetirementList([DataSourceRequest] DataSourceRequest request, Cat_TypeOfRetirementSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_TypeOfRetirementSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TypeOfRetirement);
        }

        public ActionResult ExportOfRetirementSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_CAT_SP_GET_NAMEENTITYBYIDs);
        }

        public JsonResult GetMultiTypeOfRetirement(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TypeOfRetirement_Multi);
        }

        #endregion


        public JsonResult GetMultiRank(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Rank_Multi);
        }

        [HttpPost]
        public ActionResult GetTypeOfTransferList([DataSourceRequest] DataSourceRequest request, Cat_TypeOfTransferSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_TypeOfTransferSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TypeOfTransfer);
        }

        public JsonResult GetMultiTypeOfTransfer(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TypeOfTransfer_Multi);
        }

        public JsonResult GetMultiTypeOfTransferNew(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TypeOfTransferNew_Multi);
        }

        public JsonResult GetMultiTypeOfTransferTransfer(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TypeOfTransferTransfer_Multi);
        }

        public ActionResult ExportAllTypeOfTransferList([DataSourceRequest] DataSourceRequest request, Cat_TypeOfTransferSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_TypeOfTransferSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TypeOfTransfer);
        }

        public ActionResult ExportTypeOfTransferSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_hr_sp_get_TypeOfTransferByIds);
        }

        public JsonResult GetEducationLevelOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_EducationLevel_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatNameEntityMultiModel> listModel = listEntity.Translate<CatNameEntityMultiModel>();
                listModel = listModel.OrderBy(s => s.NameEntityName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGraduatedLevelOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_GraduatedLevel_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatNameEntityMultiModel> listModel = listEntity.Translate<CatNameEntityMultiModel>();
                listModel = listModel.OrderBy(s => s.NameEntityName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiEducationLevel(string text)
        {
            string status = string.Empty;
            var services = new ActionService(LanguageCode);
            var obj = new List<object>();
            obj.AddRange(new object[3]);
            obj[0] = text;
            obj[1] = 1;
            obj[2] = int.MaxValue - 1;
            var lstGraduatedLevel = baseService.GetData<Cat_GraduatedLevelMultiEntity>(obj, ConstantSql.hrm_cat_sp_get_EducationLevel, UserLogin, ref status);
            if (lstGraduatedLevel != null)
            {
                lstGraduatedLevel = lstGraduatedLevel.OrderBy(s => s.Order).ThenBy(s => s.NameEntityName).ToList();
            }
            return Json(lstGraduatedLevel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckMoreHeightSchool(Guid ID)
        {
            var Mes = NotificationType.Error.ToString();
            var ser = new ActionService(UserLogin, LanguageCode);
            var status = string.Empty;
            var Data = ser.GetData<Cat_NameEntityEntity>(ID, ConstantSql.hrm_cat_sp_get_NameEntityById, ref status);
            if (Data != null && Data.Count > 0)
            {
                if (Data[0].EnumType == E_MOREHEIGHT.E_MOREHEIGHT.ToString())
                {

                    return Json(NotificationType.Success.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(Mes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiDiseList(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Dise_Multi);
        }

        public JsonResult GetMultiCategoryKPI(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_CategoryKPI_Multi);
        }

        public JsonResult GetMultiGroupKPI(string text)
        {
            return GetDataForControl<Eva_KPIGroupMultiModel, Eva_KPIGroupMultiModel>(text, ConstantSql.hrm_cat_sp_get_KPIGroup_Multi);
        }

        #endregion

        #region Cat_ResignReason
        /// <summary>
        /// [Son.Vo] - Lấy danh sách dữ liệu bảng TAMScanReasonMiss (Cat_TAMScanReasonMiss)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTAMScanReasonMissList([DataSourceRequest] DataSourceRequest request, Cat_TAMScanReasonMissSearchModel model)
        {
            return GetListDataAndReturn<Cat_TAMScanReasonMissModel, Cat_TAMScanReasonMissEntity, Cat_TAMScanReasonMissSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_TAMScanReasonMiss);
        }
        public ActionResult ExportAllTAMScanReasonMisslList([DataSourceRequest] DataSourceRequest request, Cat_TAMScanReasonMissSearchModel model)
        {
            return ExportAllAndReturn<Cat_TAMScanReasonMissEntity, Cat_TAMScanReasonMissModel, Cat_TAMScanReasonMissSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TAMScanReasonMiss);
        }

        public ActionResult ExportTAMScanReasonMissSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_TAMScanReasonMissEntity, Cat_TAMScanReasonMissModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_TAMScanReasonMissByIds);
        }



        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_ResignReasonMiss(Cat_ResignReason) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllResignReasonMissList([DataSourceRequest] DataSourceRequest request, Cat_TAMScanReasonMissSearchModel model)
        {
            return ExportAllAndReturn<Cat_TAMScanReasonMissEntity, Cat_TAMScanReasonMissModel, Cat_TAMScanReasonMissSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TAMScanReasonMiss);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_ResignReasonMiss (Cat_ResignReason) theo điều kiện tìm kiếm
        public ActionResult ExportResignReasonMissSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_TAMScanReasonMissEntity, Cat_TAMScanReasonMissModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }

        #endregion

        #region Cat_OvertimeReason

        #endregion
        public ActionResult GetOvertimeReasonList([DataSourceRequest] DataSourceRequest request, Cat_OvertimeResonSearchModel model)
        {
            return GetListDataAndReturn<Cat_OvertimeResonModel, Cat_OvertimeResonEntity, Cat_OvertimeResonSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_OvertimeReason);
        }
        public ActionResult ExportAllOvertimeReasonlList([DataSourceRequest] DataSourceRequest request, Cat_OvertimeResonSearchModel model)
        {
            return ExportAllAndReturn<Cat_OvertimeResonEntity, Cat_OvertimeResonModel, Cat_OvertimeResonSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_OvertimeReason);
        }

        public ActionResult ExportOvertimeReasonSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_OvertimeResonEntity, Cat_OvertimeResonModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_OvertimeReasonByIds);
        }
        #region
        /// <summary>
        /// [Quan.nguyen] - Lấy danh sách dữ liệu bảng ResignReason (Cat_ResignReason)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetResignReasonList([DataSourceRequest] DataSourceRequest request, CatResignReasonSearchModel model)
        {
            return GetListDataAndReturn<CatResignReasonModel, Cat_ResignReasonEntity, CatResignReasonSearchModel>
                (request, model, ConstantSql.hrm_cat_sp_get_ResignReason);
        }
        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_ResignReason(Cat_ResignReason) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllResignReasonList([DataSourceRequest] DataSourceRequest request, CatResignReasonSearchModel model)
        {
            return ExportAllAndReturn<Cat_ResignReasonEntity, CatResignReasonModel, CatResignReasonSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ResignReason);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_ResignReason (Cat_ResignReason) theo điều kiện tìm kiếm
        public ActionResult ExportResignReasonSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ResignReasonEntity, CatResignReasonModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        #endregion

        #region Cat_Region
        public JsonResult GetMultiRegion(string text)
        {
            return GetDataForControl<CatRegionMultiModel, Cat_RegionMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Region_multi);
        }
        #endregion

        #region Cat_UsualAllowance
        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Trợ Cấp (Cat_UsualAllowance)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUsualAllowanceList([DataSourceRequest] DataSourceRequest request, Cat_UsualAllowanceSearchModel model)
        {
            return GetListDataAndReturn<Cat_UsualAllowanceModel, Cat_UsualAllowanceEntity, Cat_UsualAllowanceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UsualAllowance);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu choTrợ Cấp (Cat_UsualAllowance) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllUsualAllowanceList([DataSourceRequest] DataSourceRequest request, Cat_UsualAllowanceSearchModel model)
        {
            return ExportAllAndReturn<Cat_UsualAllowanceEntity, Cat_UsualAllowanceModel, Cat_UsualAllowanceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UsualAllowance);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của  Trợ Cấp (Cat_UsualAllowance) theo điều kiện tìm kiếm
        public ActionResult ExportUsualAllowanceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_UsualAllowanceEntity, Cat_UsualAllowanceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_UsualAllowanceByIds);
        }

        public JsonResult GetMultiUsualAllowance(string text)
        {
            return GetDataForControl<Cat_UsualAllowanceMultiModel, Cat_UsualAllowanceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UsualAllowance_Multi);
        }

        public JsonResult GetUsualAllowanceDataByID(Guid AllowanceID)
        {
            var _UsualAllowanceServices = new Cat_UsualAllowanceServices();
            var entity = _UsualAllowanceServices.GetUsualAllowanceDataByID(AllowanceID);
            return Json(entity);
        }


        #endregion

        #region Cat_LineItem
        [HttpPost]
        public ActionResult GetLineItemList([DataSourceRequest] DataSourceRequest request, Cat_LineItemSearchModel model)
        {
            return GetListDataAndReturn<Cat_LineItemModel, Cat_LineItemEntity, Cat_LineItemSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LineItem);
        }
        public JsonResult GetMultiLineItem(string text)
        {
            return GetDataForControl<Cat_LineItemMultiModel, Cat_LineItemMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LineItem_Multi);
        }
        public ActionResult ExportAllLineItemlList([DataSourceRequest] DataSourceRequest request, Cat_LineItemSearchModel model)
        {
            return ExportAllAndReturn<Cat_LineItemEntity, Cat_LineItemModel, Cat_LineItemSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LineItem);
        }

        public ActionResult ExportLineItemSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_LineItemEntity, Cat_LineItemModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LineItemByIds);
        }

        #endregion

        #region Cat_Item
        public ActionResult GetItemList([DataSourceRequest] DataSourceRequest request, Cat_ItemSearchModel model)
        {
            return GetListDataAndReturn<Cat_ItemEntity, Cat_ItemModel, Cat_ItemSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Item);
        }
        public JsonResult GetMultiItem(string text)
        {
            return GetDataForControl<Cat_ItemModel, Cat_ItemEntity>(text, ConstantSql.hrm_cat_sp_get_Item_Multi);
        }

        public ActionResult ExportAllItemlList([DataSourceRequest] DataSourceRequest request, Cat_ItemSearchModel model)
        {
            return ExportAllAndReturn<Cat_ItemEntity, Cat_ItemModel, Cat_ItemSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Item);
        }

        public ActionResult ExportItemSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ItemEntity, Cat_ItemModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ItemByIds);
        }

        #endregion

        #region Cat_Brand
        public JsonResult GetMultiBrand(string text)
        {
            return GetDataForControl<Cat_BrandMultiModel, Cat_BrandMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Brand_Multi);
        }
        public ActionResult ExportAllBrandList([DataSourceRequest] DataSourceRequest request, Cat_BrandSearchModel model)
        {
            return ExportAllAndReturn<Cat_BrandEntity, Cat_BrandModel, Cat_BrandSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Brand);
        }


        public ActionResult ExportBrandSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_BrandEntity, Cat_BrandModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_BrandByIds);
        }

        public ActionResult GetBrandList([DataSourceRequest] DataSourceRequest request, Cat_BrandSearchModel model)
        {
            return GetListDataAndReturn<Cat_BrandModel, Cat_BrandEntity, Cat_BrandSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Brand);
        }
        #endregion

        #region Cat_Unit
        public JsonResult GetMultiUnit(string text)
        {
            return GetDataForControl<Cat_UnitMultiModel, Cat_UnitMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Unit_Multi);
        }
        public ActionResult GetUnitList([DataSourceRequest] DataSourceRequest request, Cat_UnitSearchModel model)
        {
            return GetListDataAndReturn<Cat_UnitModel, Cat_UnitEntity, Cat_UnitSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Unit);
        }

        public ActionResult ExportAllUnitlList([DataSourceRequest] DataSourceRequest request, Cat_UnitSearchModel model)
        {
            return ExportAllAndReturn<Cat_UnitEntity, Cat_UnitModel, Cat_UnitSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Unit);
        }

        public ActionResult ExportUnitSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_UnitEntity, Cat_UnitModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_UnitByIds);
        }


        #endregion

        #region Cat_KBIBonus
        public JsonResult GetMultiKPIBonus(string text)
        {
            return GetDataForControl<Cat_KPIBonusMultiModel, Cat_KPIBonusMultiEntity>(text, ConstantSql.hrm_cat_sp_get_KPIBonus_Multi);
        }
        public ActionResult ExportAllKPIList([DataSourceRequest] DataSourceRequest request, Cat_KPIBonusSearchMoel model)
        {
            return ExportAllAndReturn<Cat_KPIBonusEntity, Cat_KPIBonusModel, Cat_KPIBonusSearchMoel>(request, model, ConstantSql.hrm_cat_sp_get_KPIBonus);
        }

        public ActionResult ExportAllKPIItemList([DataSourceRequest] DataSourceRequest request, Cat_KPIBonusItemSearchMoel model)
        {
            return ExportAllAndReturn<Cat_KPIBonusItemEntity, Cat_KPIBonusItemModel, Cat_KPIBonusItemSearchMoel>(request, model, ConstantSql.hrm_cat_sp_get_KPIBonusItem);
        }


        public ActionResult ExportKPIBonusSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_KPIBonusEntity, Cat_KPIBonusModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_KPIBonusIds);
        }
        public ActionResult ExportKPIBonusItemSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_KPIBonusItemEntity, Cat_KPIBonusItemModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_KPIBonusItemByIds);
        }


        public ActionResult GetKPIBonusList([DataSourceRequest] DataSourceRequest request, Cat_KPIBonusSearchMoel model)
        {
            return GetListDataAndReturn<Cat_KPIBonusModel, Cat_KPIBonusEntity, Cat_KPIBonusSearchMoel>(request, model, ConstantSql.hrm_cat_sp_get_KPIBonus);
        }
        public ActionResult GetKPIBonusItemList([DataSourceRequest] DataSourceRequest request, Cat_KPIBonusItemSearchMoel model)
        {
            return GetListDataAndReturn<Cat_KPIBonusItemModel, Cat_KPIBonusItemEntity, Cat_KPIBonusItemSearchMoel>(request, model, ConstantSql.hrm_cat_sp_get_KPIBonusItem);
        }
        #endregion

        #region Cat_UsualAllowanceLevel
        public ActionResult GetUsualAllowanceLevelByAllowanceID([DataSourceRequest] DataSourceRequest request, Guid AllowanceID)
        {
            try
            {
                string status = string.Empty;
                var baseService = new BaseService();
                var objs = new List<object>();
                objs.Add(AllowanceID);
                var result = baseService.GetData<Cat_UsualAllowanceLevelEntity>(objs, ConstantSql.hrm_cat_sp_get_UsualAllowanceLevelByAllowanceID, UserLogin, ref status);
                return Json(result.ToDataSourceResult(request));
            }
            catch
            {

            }
            return Json(null);
        }
        #endregion

        #region Cat_SalaryClass
        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng mã lương (Cat_SalaryClass)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSalaryClassList([DataSourceRequest] DataSourceRequest request, Cat_SalaryClassSearchModel model)
        {
            return GetListDataAndReturn<Cat_SalaryClassModel, Cat_SalaryClassEntity, Cat_SalaryClassSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryClass);
        }


        [HttpPost]
        public ActionResult GetDataRankDetailByOrderNumber(string OrderNumber, string rankCode)
        {

            string status = string.Empty;

            var rankDetailServices = new Cat_SalaryClassServices();
            var objRankDetail = new List<object>();
            objRankDetail = Common.AddRange(7);
            var lstRankDetail = rankDetailServices.GetData<Cat_SalaryRankEntity>(objRankDetail, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status);

            if (lstRankDetail.Count > 0)
            {
                var entity = lstRankDetail.Where(s => s.Code == rankCode && s.OrderNumber == int.Parse(OrderNumber + 1)).FirstOrDefault();
                return Json(entity, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public ActionResult GetDataRankByOrderNumber(string OrderNumber)
        {

            string status = string.Empty;

            var rankServices = new Cat_SalaryClassServices();
            var objRank = new List<object>();
            objRank.Add(null);
            objRank.Add(1);
            objRank.Add(int.MaxValue - 1);
            var lstRank = rankServices.GetData<Cat_SalaryClassEntity>(objRank, ConstantSql.hrm_cat_sp_get_SalaryClass, UserLogin, ref status);

            var rankDetailServices = new Cat_SalaryClassServices();
            var objRankDetail = new List<object>();
            objRankDetail = Common.AddRange(7);
            var lstRankDetail = rankDetailServices.GetData<Cat_SalaryRankEntity>(objRankDetail, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status);

            if (lstRank.Count > 0)
            {
                var entity = lstRank.Where(s => s.OrderNumber == int.Parse(OrderNumber + 1)).FirstOrDefault();
                if (entity != null)
                {
                    var lstRankDetailByRankID = lstRankDetail.Where(s => s.SalaryClassID == entity.ID).ToList();
                    int total = lstRankDetailByRankID.Count;
                    int count = lstRankDetailByRankID.Count - 1;
                    var orderNumber = total - count;
                    var rankDetailEntiy = lstRankDetail.Where(s => s.OrderNumber == orderNumber && entity.ID == s.SalaryClassID).FirstOrDefault();
                    return Json(rankDetailEntiy, JsonRequestBehavior.AllowGet);
                }

            }

            return null;
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho mã lương (Cat_SalaryClass) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllSalaryClassList([DataSourceRequest] DataSourceRequest request, Cat_SalaryClassSearchModel model)
        {
            return ExportAllAndReturn<Cat_SalaryClassEntity, Cat_SalaryClassModel, Cat_SalaryClassSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryClass);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của mã lương (Cat_SalaryClass) theo điều kiện tìm kiếm
        public ActionResult ExportSalaryClassSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SalaryClassEntity, Cat_SalaryClassModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        [HttpPost]
        public JsonResult GetMultiSalaryClass(string text)
        {
            return GetDataForControl<Cat_SalaryClassMultiModel, Cat_SalaryClassMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalaryClass_multi);
        }

        public JsonResult GetMultiSalaryJobGroup(string text)
        {
            return GetDataForControl<Cat_SalaryJobGroupMultiModel, Cat_SalaryJobGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalaryJobGroup_multi);
        }

        public JsonResult GetMultiSalaryAdjCampaign(string text)
        {
            return GetDataForControl<Cat_SalAdjustmentCampaignMultiModel, Cat_SalAdjustmentCampaignMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalaryAdjCampaign_multi);
        }

        #endregion
        #region Cat_Cost
        [HttpPost]
        public ActionResult GetCat_CostList([DataSourceRequest] DataSourceRequest request, Cat_CostSearchModel model)
        {
            return GetListDataAndReturn<Cat_CostModel, Cat_CostEntity, Cat_CostSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_CostList);
        }
        public ActionResult ExportAllCat_CostList([DataSourceRequest] DataSourceRequest request, Cat_CostSearchModel model)
        {
            return ExportAllAndReturn<Cat_CostEntity, Cat_CostModel, Cat_CostSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_CostList);
        }
        public ActionResult ExportCat_CostSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CostEntity, Cat_CostModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_Cat_CostByIds);
        }
        [HttpPost]
        public JsonResult GetMultiCat_Cost(string text)
        {
            return GetDataForControl<Cat_CostMultiModel, Cat_CostMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Cat_Cost_multi);
        }
        #endregion
        #region  Cat_PolicyCostsProfile
        [HttpPost]
        public ActionResult GetCat_PolicyCostsProfileList([DataSourceRequest] DataSourceRequest request, Cat_PolicyCostsProfileSearchModel model)
        {
            return GetListDataAndReturn<Cat_PolicyCostsProfileModel, Cat_PolicyCostsProfileEntity, Cat_PolicyCostsProfileSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsProfileList);
        }
        public ActionResult ExportAllCat_PolicyCostsProfileList([DataSourceRequest] DataSourceRequest request, Cat_PolicyCostsProfileSearchModel model)
        {
            return ExportAllAndReturn<Cat_PolicyCostsProfileEntity, Cat_PolicyCostsProfileModel, Cat_PolicyCostsProfileSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsProfileList);
        }
        public ActionResult ExportCat_PolicyCostsProfileSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PolicyCostsProfileEntity, Cat_PolicyCostsProfileModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsProfileByIds);
        }
        [HttpPost]
        public ActionResult PolicyCostsProfileValidate(Cat_PolicyCostsProfileModel model)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(1);
            objs.Add(int.MaxValue - 1);
            objs.Add(UserLogin);
            var result = baseService.GetData<Cat_PolicyCostsProfileEntity>(objs, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsProfileList, UserLogin, ref status);
            if (result != null)
            {
                if (model != null && !string.IsNullOrEmpty(model.lstProfileID))
                {
                    var listProfileID = model.lstProfileID.Split(',');
                    foreach (var proID in listProfileID)
                    {
                        if (!string.IsNullOrEmpty(proID))
                            model.ProfileID = Guid.Parse(proID);
                        var objDuplicate = result.Where(x => x.ProfileID == model.ProfileID && x.DateEffective == model.DateEffective && x.CostID == model.CostID && ((model.ID != Guid.Empty && model.ID != null && x.ID != model.ID) || model.ID == null)).FirstOrDefault();
                        if (objDuplicate != null)
                        {
                            status = ConstantDisplay.HRM_HR_Cat_PolicyCostsProfile_Duplicate.TranslateString();
                        }
                    }
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region  Cat_PolicyCosts
        [HttpPost]
        public ActionResult GetCat_PolicyCostsList([DataSourceRequest] DataSourceRequest request, Cat_PolicyCostsSearchModel model)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var listPolicyCostsResult = new List<Cat_PolicyCostsEntity>();
            bool isSearch = true;
            var objs = new List<object>();
            objs.Add(model.DateEffectiveFrom);
            objs.Add(model.DateEffectiveTo);
            objs.Add(model.CostIDs);
            objs.Add(model.RateIncreaseFrom);
            objs.Add(model.RateIncreaseTo);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(1);
            objs.Add(int.MaxValue - 1);
            var listPolicyCosts = baseService.GetData<Cat_PolicyCostsEntity>(objs, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsList, UserLogin, ref status);
            if (string.IsNullOrEmpty(model.Orgstructures) && string.IsNullOrEmpty(model.JobTitles) && string.IsNullOrEmpty(model.WorkPlaces)
                && string.IsNullOrEmpty(model.EmployeeTypes) && string.IsNullOrEmpty(model.CostCentres) && string.IsNullOrEmpty(model.SalaryClasses) && string.IsNullOrEmpty(model.Regions))
            {
                isSearch = false;
                listPolicyCostsResult = listPolicyCosts;
            }
            if (listPolicyCosts != null && listPolicyCosts.Count > 0 && isSearch)
            {
                var arrOrgstructures = new string[] { };
                var arrJobTitles = new string[] { };
                var arrWorkPlaces = new string[] { };
                var arrEmployeeTypes = new string[] { };
                var arrCostCentres = new string[] { };
                var arrSalaryClasses = new string[] { };
                var arrRegions = new string[] { };
                if (!string.IsNullOrEmpty(model.Orgstructures))
                {
                    arrOrgstructures = (model.Orgstructures).Split(',');
                }
                if (!string.IsNullOrEmpty(model.JobTitles))
                {
                    arrJobTitles = (model.JobTitles).Split(',');
                }
                if (!string.IsNullOrEmpty(model.WorkPlaces))
                {
                    arrWorkPlaces = (model.WorkPlaces).Split(',');
                }
                if (!string.IsNullOrEmpty(model.EmployeeTypes))
                {
                    arrEmployeeTypes = (model.EmployeeTypes).Split(',');
                }
                if (!string.IsNullOrEmpty(model.CostCentres))
                {
                    arrCostCentres = (model.CostCentres).Split(',');
                }
                if (!string.IsNullOrEmpty(model.SalaryClasses))
                {
                    arrSalaryClasses = (model.SalaryClasses).Split(',');
                }
                if (!string.IsNullOrEmpty(model.Regions))
                {
                    arrRegions = (model.Regions).Split(',');
                }
                bool hasData = false;
                foreach (var objPolicyCosts in listPolicyCosts)
                {
                    hasData = false;
                    if (arrOrgstructures.Count() > 0)
                    {
                        foreach (var org in arrOrgstructures)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.OrgStructureIDs) && objPolicyCosts.OrgStructureIDs.Contains(org))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }
                    if (hasData)
                        continue;
                    if (arrJobTitles.Count() > 0)
                    {
                        foreach (var job in arrJobTitles)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.JobTitleIDs) && objPolicyCosts.JobTitleIDs.Contains(job))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }
                    if (hasData)
                        continue;
                    if (arrWorkPlaces.Count() > 0)
                    {
                        foreach (var work in arrWorkPlaces)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.WorkPlaceIDs) && objPolicyCosts.WorkPlaceIDs.Contains(work))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }
                    if (hasData)
                        continue;
                    if (arrEmployeeTypes.Count() > 0)
                    {
                        foreach (var emp in arrEmployeeTypes)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.EmployeeTypeIDs) && objPolicyCosts.EmployeeTypeIDs.Contains(emp))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }
                    if (hasData)
                        continue;
                    if (arrCostCentres.Count() > 0)
                    {
                        foreach (var cost in arrCostCentres)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.CostCentreIDs) && objPolicyCosts.CostCentreIDs.Contains(cost))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }
                    if (hasData)
                        continue;
                    if (arrSalaryClasses.Count() > 0)
                    {
                        foreach (var sal in arrSalaryClasses)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.SalaryClassIDes) && objPolicyCosts.SalaryClassIDes.Contains(sal))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }
                    if (hasData)
                        continue;
                    if (arrRegions.Count() > 0)
                    {
                        foreach (var reg in arrRegions)
                        {
                            if (!string.IsNullOrEmpty(objPolicyCosts.RegionIDs) && objPolicyCosts.RegionIDs.Contains(reg))
                            {
                                listPolicyCostsResult.Add(objPolicyCosts);
                                hasData = true;
                                break;
                            }
                        }
                    }

                }
            }
            if (listPolicyCostsResult != null)
            {
                request.Page = 1;
                VnResource.Helper.Utility.LanguageHelper.LanguageCode = LanguageCode;
                var lstResulFirst = listPolicyCostsResult.ToList();
                var dataSourceResult = lstResulFirst.ToDataSourceResult(request);
                if (listPolicyCostsResult.FirstOrDefault().GetPropertyValue("TotalRow") != null)
                {
                    dataSourceResult.Total = listPolicyCostsResult.Count() <= 0 ? 0 : (int)listPolicyCostsResult.FirstOrDefault().GetPropertyValue("TotalRow");
                }

                var serializer = new JavaScriptSerializer();
                var result = new ContentResult();
                serializer.MaxJsonLength = Int32.MaxValue; // Whatever max length you want here
                result.Content = serializer.Serialize(dataSourceResult);
                result.ContentType = "application/json";
                return result;
            }
            var listModelNull = new List<Cat_PolicyCostsModel>();
            ModelState.AddModelError("Id", status);
            return Json(listModelNull.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ExportAllCat_PolicyCostsList([DataSourceRequest] DataSourceRequest request, Cat_PolicyCostsSearchModel model)
        {
            return ExportAllAndReturn<Cat_PolicyCostsEntity, Cat_PolicyCostsModel, Cat_PolicyCostsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsList);
        }
        public ActionResult ExportCat_PolicyCostsSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PolicyCostsEntity, Cat_PolicyCostsModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsByIds);
        }
        //[HttpPost]
        //public ActionResult PolicyCostsProfileValidate(Cat_PolicyCostsModel model)
        //{
        //    string status = string.Empty;
        //    var baseService = new BaseService();
        //    var objs = new List<object>();
        //    objs.Add(null);
        //    objs.Add(null);
        //    objs.Add(null);
        //    objs.Add(null);
        //    objs.Add(null);
        //    objs.Add(null);
        //    objs.Add(1);
        //    objs.Add(int.MaxValue - 1);
        //    objs.Add(UserLogin);
        //    var result = baseService.GetData<Cat_PolicyCostsEntity>(objs, ConstantSql.hrm_cat_sp_get_Cat_PolicyCostsList, UserLogin, ref status);
        //    if (result != null)
        //    {
        //        if (model != null && !string.IsNullOrEmpty(model.lstProfileID))
        //        {
        //            var listProfileID = model.lstProfileID.Split(',');
        //            foreach (var proID in listProfileID)
        //            {
        //                if (!string.IsNullOrEmpty(proID))
        //                    model.ProfileID = Guid.Parse(proID);
        //                var objDuplicate = result.Where(x => x.ProfileID == model.ProfileID && x.DateEffective == model.DateEffective && x.CostID == model.CostID && ((model.ID != Guid.Empty && model.ID != null && x.ID != model.ID) || model.ID == null)).FirstOrDefault();
        //                if (objDuplicate != null)
        //                {
        //                    status = ConstantDisplay.HRM_HR_Cat_PolicyCosts_Duplicate.TranslateString();
        //                }
        //            }
        //        }
        //    }
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region Cat_GradePayroll
        public ActionResult GetGradePayrollList([DataSourceRequest] DataSourceRequest request, Cat_GradePayrollSearchlModel model)
        {
            return GetListDataAndReturn<Cat_GradePayrollModel, Cat_GradePayrollEntity, Cat_GradePayrollSearchlModel>
                (request, model, ConstantSql.hrm_cat_sp_get_GradePayroll);
        }

        [HttpPost]
        public ActionResult GetElementByGradePayrollID([DataSourceRequest] DataSourceRequest request, Guid payrollID)
        {

            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(payrollID);
            var result = baseService.GetData<Cat_ElementEntity>(objs, ConstantSql.hrm_cat_sp_get_ElementByPayrollID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult GetAdvancePaymentByGradePayrollID([DataSourceRequest] DataSourceRequest request, Guid payrollID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(payrollID);
            var result = baseService.GetData<Cat_ElementEntity>(objs, ConstantSql.hrm_cat_sp_get_AdvancePaymentByPayrollID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Cat_PerformanceType
        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Loại Đánh Giá (Cat_PerformanceType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPerformanceTypeList([DataSourceRequest] DataSourceRequest request, Cat_PerformanceTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_PerformanceTypeModel, Cat_PerformanceTypeEntity, Cat_PerformanceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PerformanceType);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho mã lương (Cat_SalaryClass) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllPerformanceTypeList([DataSourceRequest] DataSourceRequest request, Cat_PerformanceTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_PerformanceTypeEntity, Cat_PerformanceTypeModel, Cat_PerformanceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PerformanceType);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của mã lương (Cat_SalaryClass) theo điều kiện tìm kiếm
        public ActionResult ExportPerformanceTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PerformanceTypeEntity, Cat_PerformanceTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PerformanceTypeByIds);
        }

        public JsonResult GetMultiPerformanceType(string text)
        {
            return GetDataForControl<Cat_PerformanceTypeModel, Cat_PerformanceTypeEntity>(text, ConstantSql.hrm_cat_sp_get_PerformanceType_multi);
        }
        #endregion

        #region Cat_ProductItem

        public ActionResult GetProductItemList([DataSourceRequest] DataSourceRequest request, Cat_ProductItemSearchModel model)
        {
            return GetListDataAndReturn<Cat_ProductItemModel, Cat_ProductItemEntity, Cat_ProductItemSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductItem_All);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_ProductItem([DataSourceRequest] DataSourceRequest request, Cat_ProductItemModel product)
        {
            return Json("");
        }

        public ActionResult GetProductItemPriceList([DataSourceRequest] DataSourceRequest request, Cat_ProductItemPriceSearchModel model)
        {
            return GetListDataAndReturn<Cat_ProductItemPriceModel, Cat_ProductItemPriceEntity, Cat_ProductItemPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductItemPrice);
        }

        public ActionResult ExportAllProductItemPriceList([DataSourceRequest] DataSourceRequest request, Cat_ProductItemPriceSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProductItemPriceModel, Cat_ProductItemPriceEntity, Cat_ProductItemPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductItemPrice);
        }

        public ActionResult ExportProductItemPriceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProductItemPriceEntity, Cat_ProductItemPriceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ProductItemPriceByIds);
        }

        public ActionResult GetProductItemNormsPriceList([DataSourceRequest] DataSourceRequest request, Cat_ProductItemNormsPriceSearchModel model)
        {
            return GetListDataAndReturn<Cat_ProductItemNormsPriceModel, Cat_ProductItemNormsPriceEntity, Cat_ProductItemNormsPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductItemNormsPrice);
        }
        public ActionResult ExportAllProductItemList([DataSourceRequest]DataSourceRequest request, Cat_ProductItemSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProductItemEntity, Cat_ProductItemModel, Cat_ProductItemSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductItem_All);
        }

        public ActionResult ExportProductItemSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProductItemEntity, Cat_ProductItemModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ProductItemByIds);
        }
        #endregion

        #region Cat_Product
        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Đơn Giá SP (Cat_Product)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetProductList([DataSourceRequest] DataSourceRequest request, CatProductSearchModel model)
        {
            return GetListDataAndReturn<CatProductModel, Cat_ProductEntity, CatProductSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Product);
        }

        public JsonResult GetMultiProduct(string text)
        {
            return GetDataForControl<Cat_ProductMultiModel, Cat_ProductMultiEntity>(text, ConstantSql.hrm_Cat_sp_get_Product_multi);
        }

        public JsonResult GetProductMulti()
        {
            //GetDataForControl<CatProductModel,Cat_ProductEntity>( ConstantSql.hrm_cat_sp_get_Product)
            List<object> listModel = new List<object>();
            listModel.AddRange(new object[6]);
            listModel[4] = 1;
            listModel[5] = Int32.MaxValue - 1;
            return GetData<Cat_ProductMultiModel, Cat_ProductMultiEntity>(listModel, ConstantSql.hrm_cat_sp_get_Product);
        }



        public JsonResult GetProductITemMulti(Guid? ID, string text)
        {
            if (ID != null)
            {
                List<object> listModel = new List<object>();
                listModel.AddRange(new object[3]);
                listModel[0] = ID;
                listModel[1] = 1;
                listModel[2] = int.MaxValue - 1;

                string status = string.Empty;
                var service = new BaseService();
                var listEntity = service.GetData<Cat_ProductItemEntity>(listModel, ConstantSql.hrm_cat_sp_get_ProductItem, UserLogin, ref status);
                if (string.IsNullOrEmpty(text))
                {
                    return Json(listEntity);
                }
                else
                {
                    var result = listEntity.Where(m => (m.ProductItemName != null && m.ProductItemName.ToUpper().Contains(text.ToUpper())) || (m.Code != null) && m.Code.ToUpper().Contains(text.ToUpper()));
                    return Json(result.ToList());
                }

                //return GetData<Cat_ProductItemModel, Cat_ProductItemEntity>(listModel, ConstantSql.hrm_cat_sp_get_ProductItem);
            }
            return Json(null);
        }

        public JsonResult GetFieldterProductITemMulti(string text)
        {
            //List<object> listModel = new List<object>();
            //listModel.AddRange(new object[2]);
            //listModel[1] = text;
            //return GetData<Cat_ProductItemModel, Cat_ProductItemEntity>(listModel, ConstantSql.hrm_cat_sp_get_ProductItem_Multi);
            return GetDataForControl<Cat_ProductItemMultiModel, Cat_ProductItemMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ProductItem_Multi);
        }

        public JsonResult GetProductItemByProductMulti(string text, string _productID)
        {

            var service = new BaseService();
            List<object> listModel = new List<object>();
            string status = string.Empty;
            listModel = Common.AddRange(3);
            if (!string.IsNullOrEmpty(_productID))
            {
                listModel[0] = Common.DotNetToOracle(_productID);
            }
            var listResult = service.GetData<Cat_ProductItemEntity>(listModel, ConstantSql.hrm_cat_sp_get_ProductItem, UserLogin, ref status);

            if (!string.IsNullOrEmpty(text))
            {
                text = text.ToLower();
                listResult = listResult.Where(m => m.ProductItemName != null && m.ProductItemName != string.Empty && m.ProductItemName.ToString().ToLower().Contains(text)
                                               || m.Code != null && m.Code != string.Empty && m.Code.Contains(text)).ToList();
            }

            return Json(listResult.Select(m => new { m.ID, m.ProductItemName }));
        }
        public JsonResult GetProductItemMultiV2(string text)
        {
            return GetDataForControl<Cat_ProductItemMultiModel, Cat_ProductItemMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ProductItem_Multi);
        }

        public JsonResult GetFieldterProductItemByProductMulti(string ProductID)
        {
            var service = new BaseService();
            List<object> listModel = new List<object>();
            string status = string.Empty;
            listModel = Common.AddRange(3);
            var listResult = service.GetData<Cat_ProductItemEntity>(listModel, ConstantSql.hrm_cat_sp_get_ProductItem, UserLogin, ref status);
            if (!string.IsNullOrEmpty(ProductID))
            {
                return Json(listResult.Where(m => m.ProductID != null && m.ProductID.ToString() == ProductID));
            }
            else
            {
                return Json(listResult);
            }


        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Đơn Giá SP (Cat_Prodcut) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllProductList([DataSourceRequest] DataSourceRequest request, CatProductSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProductEntity, CatProductModel, CatProductSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Product);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của Đơn Giá SP (Cat_Prodcut) theo điều kiện tìm kiếm
        public ActionResult ExportSelected(List<Guid> selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProductEntity, CatProductModel>(string.Join(",", selectedIds), valueFields, ConstantSql.hrm_cat_sp_get_ProductByIds);
        }
        #endregion

        #region Cat_Bank
        /// <summary>
        /// [Chuc.Nguyen] - Lấy danh sách dữ liệu bảng Ngân Hàng (Cat_Bank)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBankList([DataSourceRequest] DataSourceRequest request, CatBankSearchModel model)
        {
            //GetDatatable<CatBankModel, Cat_BankEntity, CatBankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Bank);
            return GetListDataAndReturn<CatBankModel, Cat_BankEntity, CatBankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Bank);
        }
        public JsonResult GetMultiBank(string text)
        {
            return GetDataForControl<CatBankMultiModel, Cat_BankMultiEntity>(text, ConstantSql.hrm_Cat_sp_get_Bank_multi);
        }

        public ActionResult ExportAllBankList([DataSourceRequest] DataSourceRequest request, CatBankSearchModel model)
        {
            //return ExportAllMapping<Cat_BankEntity, CatBankModel, CatBankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Bank, new string[] { "BankCode", "BankName", "Notes", "DateCreate", "DateUpdate" });
            return ExportAllAndReturn<Cat_BankEntity, CatBankModel, CatBankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Bank);
        }
        #endregion
        #region Cat_Bank
        /// <summary>
        /// [Chuc.Nguyen] - Lấy danh sách dữ liệu bảng Ngân Hàng (Cat_Bank)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCat_LocationList([DataSourceRequest] DataSourceRequest request, Cat_LocationSearchModel model)
        {
            //GetDatatable<CatBankModel, Cat_BankEntity, CatBankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Bank);
            return GetListDataAndReturn<Cat_LocationModel, Cat_LocationModel, Cat_LocationSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Location);
        }
        #endregion

        #region Cat_TrainingPlace
        [HttpPost]
        public ActionResult GetTrainingPlaceList([DataSourceRequest] DataSourceRequest request, Cat_TrainingPlaceSearchModel model)
        {
            //GetDatatable<CatBankModel, Cat_BankEntity, CatBankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Bank);
            return GetListDataAndReturn<Cat_TrainingPlaceModel, Cat_TrainingPlaceEntity, Cat_TrainingPlaceSearchModel>(request, model, ConstantSql.cat_sp_get_TrainingPlace);
        }

        public ActionResult ExportAllTrainingPlaceList([DataSourceRequest] DataSourceRequest request, Cat_TrainingPlaceSearchModel model)
        {
            return ExportAllAndReturn<Cat_TrainingPlaceEntity, Cat_TrainingPlaceModel, Cat_TrainingPlaceSearchModel>(request, model, ConstantSql.cat_sp_get_TrainingPlace);
        }

        public JsonResult GetMultiTrainingPlace(string text)
        {
            return GetDataForControl<Cat_TrainingPlaceMultiModel, Cat_TrainingPlaceMultiEntity>(text, ConstantSql.cat_sp_get_TrainingPlacemulti);
        }

        public JsonResult GetMultiTrainingPlaceAutoComplete(string text)
        {
            return GetDataForControl<Cat_TrainingPlaceMultiModel, Cat_TrainingPlaceMultiEntity>(text, ConstantSql.cat_sp_get_TrainingPlacemultiAutoComplete);
        }
        public ActionResult ExportTrainingPlaceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_TrainingPlaceEntity, Cat_TrainingPlaceModel>(selectedIds, valueFields, ConstantSql.cat_sp_get_TrainingPlaceByIds);
        }
        #endregion

        #region Cat_ProductType
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng ProductType
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult GetProductTypeList([DataSourceRequest] DataSourceRequest request, CatProductTypeSearchModel model)
        {
            return GetListDataAndReturn<CatProductTypeModel, Cat_ProductTypeEntity, CatProductTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductType);
        }

        public ActionResult ExportAllProductTypeList([DataSourceRequest] DataSourceRequest request, CatProductTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProductTypeEntity, CatProductTypeModel, CatProductTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductType);
        }

        public ActionResult ExportProductTypeSelected(List<Guid> selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProductTypeEntity, CatProductTypeModel>(string.Join(",", selectedIds), valueFields, ConstantSql.hrm_cat_sp_get_ProductTypeByIds);
        }

        public JsonResult GetMultiProductType(string text)
        {
            return GetDataForControl<CatProductTypeMultiModel, Cat_ProductTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ProductType_Multi);
        }
        #endregion

        #region Cat_Category
        /// <summary>
        /// [Tin.Nguyen] - Lấy danh sách dữ liệu bảng Loại Thiết Bị (Cat_Category)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCategoryList([DataSourceRequest] DataSourceRequest request, CatCategorySearchModel model)
        {
            return GetListDataAndReturn<CatCategoryModel, Cat_CategoryEntity, CatCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Category);
        }

        public ActionResult ExportCatCategorySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CategoryEntity, CatCategoryModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_CatagoryByIds);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllCategoryList([DataSourceRequest] DataSourceRequest request, CatCategorySearchModel model)
        {
            return ExportAllAndReturn<Cat_CategoryEntity, CatCategoryModel, CatCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Category);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetMultiCategory(string text)
        {
            return GetDataForControl<Cat_CategoryMultiModel, Cat_CategoryEntity>(text, ConstantSql.hrm_cat_sp_get_CatCategory_Multi);
        }


        public JsonResult GetMultiHreFacility(Guid categoryID, string facilityFilter, Guid? ID, string text)
        {
            var result = new List<Hre_FacilityMultiEntiy>();
            if (ID != null && !string.IsNullOrEmpty(text))
            {
                var item = new Hre_FacilityMultiEntiy();
                item.ID = ID.Value;
                item.FacilityName = text;
                result.Add(item);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string status = string.Empty;
            if (categoryID != Guid.Empty)
            {
                //var service = new Hre_ProfileServices();
                //result = service.GetFacilities(categoryID);
                //result = result.OrderBy(s => s.FacilityName).ToList();
                var service = new Hre_ProfileServices();
                result = service.GetData<Hre_FacilityMultiEntiy>(categoryID, ConstantSql.hrm_cat_sp_get_FacilityByCategoryId, UserLogin, ref status);

                if (!string.IsNullOrEmpty(facilityFilter))
                {
                    var rs = result.Where(s => s.FacilityName != null && s.FacilityName.ToLower().Contains(facilityFilter.ToLower())).OrderBy(s => s.FacilityName).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMultiHreFacilityIssue(string categoryID)
        {
            var result = new List<Hre_FacilityMultiEntiy>();
            string status = string.Empty;
            var service = new Hre_ProfileServices();
            if (categoryID == "")
            {
                categoryID = null;
            }
            result = service.GetData<Hre_FacilityMultiEntiy>(categoryID, ConstantSql.hrm_cat_sp_get_FacilityByCategoryId, UserLogin, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetMultiHreFacilityItem(Guid facilityID, string facilityItemFilter, Guid? ID, string text)
        {
            var result = new List<Hre_FacilityItemMultiEntiy>();
            if (ID != null && !string.IsNullOrEmpty(text))
            {
                var item = new Hre_FacilityItemMultiEntiy();
                item.ID = ID.Value;
                item.FacilityItemName = text;
                result.Add(item);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string status = string.Empty;
            if (facilityID != Guid.Empty)
            {
                var service = new Hre_ProfileServices();
                result = service.GetData<Hre_FacilityItemMultiEntiy>(facilityID, ConstantSql.hrm_cat_sp_get_FacilityItemByFacilityID_Multi, UserLogin, ref status);

                if (!string.IsNullOrEmpty(facilityItemFilter))
                {
                    var rs = result.Where(s => s.FacilityItemName != null && s.FacilityItemName.ToLower().Contains(facilityItemFilter.ToLower())).OrderBy(s => s.FacilityItemName).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMultiHreFacilityIssueItem(string facilityID)
        {
            var result = new List<Hre_FacilityItemMultiEntiy>();
            string status = string.Empty;
            if (facilityID == "")
            {
                facilityID = null;
            }
            var service = new Hre_ProfileServices();
            result = service.GetData<Hre_FacilityItemMultiEntiy>(facilityID, ConstantSql.hrm_cat_sp_get_FacilityItemByFacilityID_Multi, UserLogin, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Cat_Currency
        [HttpPost]
        public ActionResult GetCurrencyList([DataSourceRequest] DataSourceRequest request, CatCurrencySearchModel model)
        {
            return GetListDataAndReturn<Cat_CurrencyModel, Cat_CurrencyEntity, CatCurrencySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Currency);
        }

        public JsonResult GetMultiCurrencyAccounting(string text)
        {
            return GetDataForControl<CatCurrencyMultiModel, Cat_CurrencyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Currency_Multi);
        }

        public JsonResult GetMultiCurrency(string text)
        {
            return GetDataForControl<CatCurrencyMultiModel, Cat_CurrencyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Currency_Multi);
        }
        public JsonResult GetMultiCurrencyVND(string text)
        {
            if (text == string.Empty || text == null)
            {
                text = "VND";
            }
            return GetDataForControl<CatCurrencyMultiModel, Cat_CurrencyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Currency_Multi);
        }
        public JsonResult GetCurrency()
        {
            var result = baseService.GetAllUseEntity<Cat_CurrencyMultiEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCatCurrencySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CurrencyEntity, Cat_CurrencyModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CurrencyByIds);
        }

        public ActionResult ExportAllCurrencyList([DataSourceRequest] DataSourceRequest request, CatCurrencySearchModel model)
        {
            return ExportAllAndReturn<Cat_CurrencyEntity, Cat_CurrencyModel, CatCurrencySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Currency);
        }
        #endregion

        #region Cat_District
        /// <summary>
        /// [Tin.Nguyen] - Lấy danh sách dữ liệu bảng Quận/Huyện (Cat_District)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDistrictList([DataSourceRequest] DataSourceRequest request, CatDistrictSearchModel model)
        {
            return GetListDataAndReturn<CatDistrictModel, Cat_DistrictEntity, CatDistrictSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_District);
        }

        public JsonResult GetDistrict()
        {
            var result = baseService.GetAllUseEntity<Cat_DistrictEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCatDistrictSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_DistrictEntity, CatDistrictModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_DistrictByIds);
        }

        public ActionResult ExportAllDistrictList([DataSourceRequest] DataSourceRequest request, CatDistrictSearchModel model)
        {
            return ExportAllAndReturn<Cat_DistrictEntity, CatDistrictModel, CatDistrictSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_District);
        }


        public JsonResult GetDistrictCascading(Guid? province, string districtFilter, Guid? ID, string text)
        {
            var result = new List<CatDistrictModel>();
            if (ID != null && !string.IsNullOrEmpty(text))
            {
                var item = new CatDistrictModel();
                item.ID = ID.Value;
                item.DistrictName = text;
                result.Add(item);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string status = string.Empty;
            if (province != Guid.Empty)
            {
                var service = new Cat_DistrictServices();
                result = service.GetData<CatDistrictModel>(province, ConstantSql.hrm_cat_sp_get_DisctrictByProvinceId, UserLogin, ref status);
                result = result.OrderBy(s => s.DistrictName).ToList();
                if (!string.IsNullOrEmpty(districtFilter))
                {
                    var rs = result.Where(s => s.DistrictName != null && s.DistrictName.ToLower().Contains(districtFilter.ToLower())).OrderBy(s => s.DistrictName).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWardCascading(Guid district, string wardFilter)
        {
            var result = new List<Cat_VillageModel>();
            string status = string.Empty;
            if (district != Guid.Empty)
            {
                var service = new Cat_VillageServices();
                result = service.GetData<Cat_VillageModel>(district, ConstantSql.hrm_cat_sp_get_VillageByDistrictId, UserLogin, ref status);
                result = result.OrderBy(s => s.DistrictName).ToList();
                if (!string.IsNullOrEmpty(wardFilter))
                {
                    var rs = result.Where(s => s.VillageName != null && s.VillageName.ToLower().Contains(wardFilter.ToLower())).OrderBy(s => s.VillageName).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_Province

        /// <summary>
        /// [Tin.Nguyen] - Lấy danh sách dữ liệu bảng Tỉnh/Thành Phố (Cat_Province)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetProvinceList([DataSourceRequest] DataSourceRequest request, CatProvinceSearchModel model)
        {
            return GetListDataAndReturn<CatProvinceModel, Cat_ProvinceEntity, CatProvinceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Province);
        }

        public JsonResult GetMultiProvince(string text)
        {
            return GetDataForControl<CatProvinceMultiModel, Cat_ProvinceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Province_multi);
        }
        public JsonResult GetMultiResignReason(string text)
        {
            return GetDataForControl<CatResignReasonModel, Cat_ResignReasonMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ResignReason_multi);
        }

        public ActionResult ExportCatProvinceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProvinceEntity, CatProvinceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ProvinceByIds);
        }

        public ActionResult ExportAllProvinceList([DataSourceRequest] DataSourceRequest request, CatProvinceSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProvinceEntity, CatProvinceModel, CatProvinceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Province);
        }

        public JsonResult GetProvince()
        {
            var result = baseService.GetAllUseEntity<Cat_DistrictEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProvinceCascading(Guid? country, string provinceFilter, Guid? ID, string text)
        {
            var result = new List<CatProvinceModel>();
            if (ID != null && !string.IsNullOrEmpty(text))
            {
                var item = new CatProvinceModel();
                item.ID = ID.Value;
                item.ProvinceName = text;
                result.Add(item);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string status = string.Empty;
            if (country != Guid.Empty)
            {
                var service = new Cat_ProvinceServices();
                result = service.GetData<CatProvinceModel>(country, ConstantSql.hrm_cat_sp_get_ProvinceByCountryId, UserLogin, ref status);
                result = result.OrderBy(s => s.ProvinceName).ToList();
                if (!string.IsNullOrEmpty(provinceFilter))
                {
                    var rs = result.Where(s => s.ProvinceName != null && s.ProvinceName.ToLower().Contains(provinceFilter.ToLower())).ToList();
                    rs = rs.OrderBy(s => s.ProvinceName).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }


            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVillageCascading(Guid? districtid, string villageFilter, Guid? ID, string text)
        {
            var result = new List<Cat_VillageModel>();
            if (ID != null && !string.IsNullOrEmpty(text))
            {
                var item = new Cat_VillageModel();
                item.ID = ID.Value;
                item.VillageName = text;
                result.Add(item);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string status = string.Empty;
            if (districtid != Guid.Empty)
            {
                var service = new Cat_ProvinceServices();
                result = service.GetData<Cat_VillageModel>(districtid, ConstantSql.hrm_cat_sp_get_VillageByDistrictId, UserLogin, ref status);
                result = result.OrderBy(s => s.VillageName).ToList();
                if (!string.IsNullOrEmpty(villageFilter))
                {
                    var rs = result.Where(s => s.VillageName != null && s.VillageName.ToLower().Contains(villageFilter.ToLower())).OrderBy(s => s.VillageName).ToList();

                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobTypeCascading(Guid RoleID, string jobtitleFilter)
        {
            var result = new List<Cat_JobTypeModel>();
            string status = string.Empty;
            if (RoleID != Guid.Empty)
            {
                var service = new Cat_ProvinceServices();
                result = service.GetData<Cat_JobTypeModel>(RoleID, ConstantSql.hrm_cat_sp_get_JobTypeByRoleId, UserLogin, ref status);
                if (!string.IsNullOrEmpty(jobtitleFilter))
                {
                    var rs = result.Where(s => s.JobTypeName != null && s.JobTypeName.ToLower().Contains(jobtitleFilter.ToLower())).ToList();

                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProvinceDataByID(Guid? provinceID)
        {
            if (provinceID != null)
            {
                ActionService actionserveice = new ActionService(UserLogin);
                string status = string.Empty;
                var _ProvinceEntity = actionserveice.GetByIdUseStore<Cat_ProvinceEntity>(provinceID.Value, ConstantSql.hrm_cat_sp_get_ProvinceById, ref status);
                if (_ProvinceEntity != null)
                    return Json(_ProvinceEntity, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion

        #region Cat_Country
        /// <summary>
        /// [Tin.Nguyen] - Lấy danh sách dữ liệu bảng Quốc Gia (Cat_Country)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCountryList([DataSourceRequest] DataSourceRequest request, CatCountrySearchModel model)
        {
            return GetListDataAndReturn<CatCountryModel, Cat_CountryEntity, CatCountrySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Country);
        }

        public JsonResult GetMultiCountry(string text)
        {
            return GetDataForControl<CatCountryMultiModel, Cat_CountryMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Country_multi);
        }
        public JsonResult GetCountryCascading(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_CountryMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Country_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatCountryMultiModel> listModel = listEntity.Translate<CatCountryMultiModel>();
                listModel = listModel.OrderBy(s => s.CountryName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMultiDistrict(string text)
        {
            return GetDataForControl<CatDistrictMultiModel, Cat_DistrictMultiEntity>(text, ConstantSql.hrm_cat_sp_get_District_multi);
        }

        [HttpPost]
        public JsonResult GetProvinceByCountry(Guid? countryID)
        {
            var result = new List<Cat_ProvinceMultiEntity>();
            if (countryID != null)
            {
                var countryServices = new Cat_CountryServices();
                result = countryServices.GetProvinceByCountry(countryID.Value);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDistrictByProvince(Guid? provinceID)
        {
            var result = new List<Cat_DistrictMultiEntity>();
            if (provinceID != null)
            {
                var countryServices = new Cat_CountryServices();
                result = countryServices.GetDistrictByProvince(provinceID.Value);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tỉnh thành, Nơi cấp CMND
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public JsonResult GetMultiVillage(string text)
        {
            return GetDataForControl<Cat_VillageMultiModel, Cat_VillageMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Village_multi);
        }

        [HttpPost]
        public JsonResult GetVillageByDistrict(Guid? districtID)
        {
            var result = new List<Cat_VillageMultiEntity>();
            if (districtID != null)
            {
                var countryServices = new Cat_CountryServices();
                result = countryServices.GetVillageByDistrict(districtID.Value);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountry()
        {
            var result = baseService.GetAllUseEntity<Cat_CountryEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ExportCatCountrySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CountryEntity, CatCountryModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CountryByIds);
        }

        public ActionResult ExportAllCountryList([DataSourceRequest] DataSourceRequest request, CatCountrySearchModel model)
        {
            return ExportAllAndReturn<Cat_CountryEntity, CatCountryModel, CatCountrySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Country);
        }

        #endregion

        #region Cat_CostCentre
        [HttpPost]
        public ActionResult GetCatCostCentreList([DataSourceRequest] DataSourceRequest request, CatCostCentreSearchModel model)
        {
            return GetListDataAndReturn<CatCostCentreModel, Cat_CostCentreEntity, CatCostCentreSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CostCentre);
        }
        /// [Phuoc.Le] - Xuất danh sách dữ liệu cho Mã chi phí (Cat_CostCentre) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllCostCentreList([DataSourceRequest] DataSourceRequest request, CatCostCentreSearchModel model)
        {
            return ExportAllAndReturn<Cat_CostCentreEntity, CatCostCentreModel, CatCostCentreSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CostCentre);
        }

        /// [Phuoc.Le] - Xuất các dòng dữ liệu được chọn của  Mã chi phí (Cat_CostCentre) theo điều kiện tìm kiếm

        public ActionResult ExportCostCentreSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CostCentreEntity, CatCostCentreModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CostCentreByIds);
        }


        public JsonResult GetMultiCostCentre(string text)
        {
            return GetDataForControl<CatCostCentreMultiModel, Cat_CostCentreMultiEntity>(text, ConstantSql.hrm_cat_sp_get_CostCentre_Multi);
        }
        public JsonResult GetMultiProject(string text)
        {
            return GetDataForControl<CatProjectMultiModel, Cat_ProjectMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Project_Multi);
        }

        public JsonResult GetCostCentre()
        {
            var result = baseService.GetAllUseEntity<Cat_CostCentreEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Cat_LeaveDayType
        [HttpPost]
        public ActionResult GetLeaveDayTypeList([DataSourceRequest] DataSourceRequest request, CatLeaveDayTypeSearchModel model)
        {
            return GetListDataAndReturn<CatLeaveDayTypeModel, Cat_LeaveDayTypeEntity, CatLeaveDayTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LeaveDayType);
        }

        public ActionResult ExportAllLeaveDayTypelList([DataSourceRequest] DataSourceRequest request, CatLeaveDayTypeSearchModel model)
        {
            //return ExportAllAndReturn<Cat_LeaveDayTypeEntity, CatLeaveDayTypeModel, CatLeaveDayTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LeaveDayType);
            #region [Vinh.Mai - 20171214] Format dữ liệu trước khi xuất excel
            string status = string.Empty;
            var lstQuyery = GetListData<CatLeaveDayTypeModel, Cat_LeaveDayTypeEntity, CatLeaveDayTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LeaveDayType, ref status);
            Dictionary<string, string> formatFields = new Dictionary<string, string>();
            formatFields.Add(CatLeaveDayTypeModel.FieldNames.SocialRate, ConstantFormat.HRM_Format_Number_Double2);
            formatFields.Add(CatLeaveDayTypeModel.FieldNames.PaidRate, ConstantFormat.HRM_Format_Number_Double2);

            status = ExportService.Export(lstQuyery, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','), formatFields);
            return Json(status);
            #endregion
        }

        public ActionResult ExportLeaveDayTypeSelected(string selectedIds, string valueFields)
        {
            //return ExportSelectedAndReturn<Cat_LeaveDayTypeEntity, CatLeaveDayTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LeaveDayTypeByIds);
            #region [Vinh.Mai - 20171214] Format dữ liệu trước khi xuất excel
            string status = string.Empty;
            var lstQuyery = GetData<CatLeaveDayTypeModel, Cat_LeaveDayTypeEntity>(selectedIds, ConstantSql.hrm_cat_sp_get_LeaveDayTypeByIds);
            Dictionary<string, string> formatFields = new Dictionary<string, string>();
            formatFields.Add(CatLeaveDayTypeModel.FieldNames.SocialRate, ConstantFormat.HRM_Format_Number_Double2);
            formatFields.Add(CatLeaveDayTypeModel.FieldNames.PaidRate, ConstantFormat.HRM_Format_Number_Double2);

            status = ExportService.Export(lstQuyery, valueFields.Split(','), formatFields);
            return Json(status);
            #endregion
        }

        public JsonResult GetMultiLeaveDayType(string text)
        {
            //return GetDataForControl<CatLeaveDayTypeMultiModel, Cat_LeaveDayTypeMultiEntity>(string.Empty, ConstantSql.hrm_cat_sp_get_LeaveDayType_multi);

            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_LeaveDayTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LeaveDayType_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatLeaveDayTypeMultiModel> listModel = listEntity.Translate<CatLeaveDayTypeMultiModel>();
                //listModel = listModel.OrderBy(s => s.LeaveDayTypeName).ToList();

                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiLeaveDayTypeInPortal(string text)
        {
            //return GetDataForControl<CatLeaveDayTypeMultiModel, Cat_LeaveDayTypeMultiEntity>(string.Empty, ConstantSql.hrm_cat_sp_get_LeaveDayType_multi);

            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_LeaveDayTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LeaveDayType_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatLeaveDayTypeMultiModel> listModel = listEntity.Translate<CatLeaveDayTypeMultiModel>();
                //[09112015][bang.nguyen][59662]
                //conbobox sử dụng Cat_LeavedayType  theo thứ tự Order (Ưu Tiên 1) và Name (Ưu Tiên 2).
                //listModel = listModel.OrderBy(s => s.LeaveDayTypeName).ToList();
                listModel = listModel.Where(s => s.NotSelectedInPortal != true).ToList();

                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_DayOff
        [HttpPost]
        public ActionResult GetDayOffList([DataSourceRequest] DataSourceRequest request, CatDayOffSearchModel model)
        {
            var services = new ActionService(UserLogin);
            string status = string.Empty;
            if (model.DateTo != null)
                model.DateTo = model.DateTo.Value.AddDays(1).AddSeconds(-1);
            List<object> listModel = new List<object>();
            listModel.AddRange(new object[5]);
            listModel[0] = model.DateFrom;
            listModel[1] = model.DateTo;
            listModel[2] = model.Type;
            listModel[3] = request.Page;
            listModel[4] = int.MaxValue - 1;

            var lstResult = services.GetData<Cat_DayOffEntity>(listModel, ConstantSql.hrm_cat_sp_get_DayOffList, ref status);
            var lstCompany = services.GetData<Cat_CompanyEntity>(string.Empty, ConstantSql.hrm_cat_sp_get_Company_multi, ref status);
            // Xử lý cho điều kiện tìm kiếm công ty chọn nhiều
            var lstResuldID = new List<Guid>();
            Guid[] lstCompanySearchID = null;
            if (!string.IsNullOrEmpty(model.CompanyIDs))
            {
                lstCompanySearchID = model.CompanyIDs.Split(',').Select(s => Guid.Parse(s)).ToArray();
            }
            if (lstCompanySearchID != null)
            {
                foreach (var company in lstCompanySearchID)
                {
                    foreach (var item in lstResult)
                    {
                        Guid[] lstCompanyID = null;
                        if (!string.IsNullOrEmpty(item.CompanyIDs))
                        {
                            lstCompanyID = item.CompanyIDs.Split(',').Select(s => Guid.Parse(s)).ToArray();
                        }
                        if (lstCompanyID != null && lstCompanyID.Contains(company))
                        {
                            if (!lstResuldID.Contains(item.ID))
                                lstResuldID.Add(item.ID);
                        }
                    }
                }

                lstResult = lstResult.Where(s => lstResuldID.Contains(s.ID)).ToList();
            }
            #region [Vinh.Mai] Lấy lên tên công ty để trả về kết quả
            foreach (var item in lstResult)
            {
                string companyName = string.Empty;
                if (!string.IsNullOrEmpty(item.CompanyIDs))
                {
                    var companys = item.CompanyIDs.Split(",").Select(s => Guid.Parse(s)).ToList();
                    var lstCompanyByResult = lstCompany.Where(s => companys.Contains(s.ID)).ToList();
                    foreach (var name in lstCompanyByResult)
                    {
                        companyName += name.CompanyName + ",";
                    }
                    item.CompanyName = companyName.Substring(0, companyName.LastIndexOf(","));
                }

            }
            #endregion

            request.Page = 1;
            var dataSourceResult = lstResult.ToDataSourceResult(request);
            //if (lstResult != null && lstResult.Count > 0)
            //{
            //    dataSourceResult.Total = lstResult.Count() <= 0 ? 0 : (int)lstResult.FirstOrDefault().TotalRow;
            //}
            return Json(dataSourceResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportAllDayOfflList([DataSourceRequest] DataSourceRequest request, CatDayOffSearchModel model)
        {
            //return ExportAllAndReturn<Cat_DayOffEntity, CatDayOffModel, CatDayOffSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DayOffList);
            var services = new ActionService(UserLogin);
            string status = string.Empty;
            if (model.DateTo != null)
                model.DateTo = model.DateTo.Value.AddDays(1).AddSeconds(-1);
            List<object> listModel = new List<object>();
            listModel.AddRange(new object[5]);
            listModel[0] = model.DateFrom;
            listModel[1] = model.DateTo;
            listModel[2] = model.Type;
            listModel[3] = 1;
            listModel[4] = int.MaxValue - 1;

            var lstResult = services.GetData<Cat_DayOffEntity>(listModel, ConstantSql.hrm_cat_sp_get_DayOffList, ref status);
            var lstCompany = services.GetData<Cat_CompanyEntity>(string.Empty, ConstantSql.hrm_cat_sp_get_Company_multi, ref status);
            // Xử lý cho điều kiện tìm kiếm công ty chọn nhiều
            var lstResuldID = new List<Guid>();
            Guid[] lstCompanySearchID = null;
            if (!string.IsNullOrEmpty(model.CompanyIDs))
            {
                lstCompanySearchID = model.CompanyIDs.Split(',').Select(s => Guid.Parse(s)).ToArray();
            }
            if (lstCompanySearchID != null)
            {
                foreach (var company in lstCompanySearchID)
                {
                    foreach (var item in lstResult)
                    {
                        Guid[] lstCompanyID = null;
                        if (!string.IsNullOrEmpty(item.CompanyIDs))
                        {
                            lstCompanyID = item.CompanyIDs.Split(',').Select(s => Guid.Parse(s)).ToArray();
                        }
                        if (lstCompanyID != null && lstCompanyID.Contains(company))
                        {
                            if (!lstResuldID.Contains(item.ID))
                                lstResuldID.Add(item.ID);
                        }
                    }
                }

                lstResult = lstResult.Where(s => lstResuldID.Contains(s.ID)).ToList();
            }
            #region [Vinh.Mai] Lấy lên tên công ty để trả về kết quả
            foreach (var item in lstResult)
            {
                string companyName = string.Empty;
                if (!string.IsNullOrEmpty(item.CompanyIDs))
                {
                    var companys = item.CompanyIDs.Split(",").Select(s => Guid.Parse(s)).ToList();
                    var lstCompanyByResult = lstCompany.Where(s => companys.Contains(s.ID)).ToList();
                    foreach (var name in lstCompanyByResult)
                    {
                        companyName += name.CompanyName + ",";
                    }
                    item.CompanyName = companyName.Substring(0, companyName.LastIndexOf(","));
                }

            }
            #endregion
            string fullPath = string.Empty;
            fullPath = ExportService.Export(lstResult, model.ValueFields.Split(','));
            return Json(fullPath);
        }

        //public ActionResult ExportDayOffSelected(string selectedIds, string valueFields)
        //{
        //    return ExportSelectedAndReturn<Cat_DayOffEntity, CatDayOffModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_DayOffByIds);
        //}
        [HttpPost]
        public ActionResult ExportDayoffSelected(string selectedIds, string valueFields)
        {
            //return ExportSelectedAndReturn<Cat_DayOffEntity, CatDayOffModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_DayOffByIds);
            var services = new ActionService(UserLogin);
            string status = string.Empty;
            var lstResult = services.GetData<Cat_DayOffEntity>(selectedIds, ConstantSql.hrm_cat_sp_get_DayOffByIds, ref status);
            var lstCompany = services.GetData<Cat_CompanyEntity>(string.Empty, ConstantSql.hrm_cat_sp_get_Company_multi, ref status);
            #region [Vinh.Mai] Lấy lên tên công ty để trả về kết quả
            foreach (var item in lstResult)
            {
                string companyName = string.Empty;
                if (!string.IsNullOrEmpty(item.CompanyIDs))
                {
                    var companys = item.CompanyIDs.Split(",").Select(s => Guid.Parse(s)).ToList();
                    var lstCompanyByResult = lstCompany.Where(s => companys.Contains(s.ID)).ToList();
                    foreach (var name in lstCompanyByResult)
                    {
                        companyName += name.CompanyName + ",";
                    }
                    item.CompanyName = companyName.Substring(0, companyName.LastIndexOf(","));
                }

            }
            #endregion

            string fullPath = string.Empty;
            fullPath = ExportService.Export(lstResult, valueFields.Split(','));
            return Json(fullPath);
        }


        #endregion

        #region Cat_LateEarlyRule
        [HttpPost]
        public ActionResult GetCatLateEarlyRuleList([DataSourceRequest] DataSourceRequest request, CatLateEarlyRuleSearchModel model)
        {
            return GetListDataAndReturn<CatLateEarlyRuleModel, Cat_LateEarlyRuleEntity, CatLateEarlyRuleSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LateEarlyRule);
        }

        public ActionResult GetCatLateEarlyRuleFormulaList([DataSourceRequest] DataSourceRequest request, CatLateEarlyRuleFormulaSearchModel model)
        {
            return GetListDataAndReturn<Cat_LateEarlyRuleFormulaModel, Cat_LateEarlyRuleFormulaEntity, CatLateEarlyRuleFormulaSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LateEarlyRule);
        }
        /// <summary>
        /// [Hien.Nguyen] 22/10/2014 - Lấy dữ liệu bảng  CatLateEarlyRule by cfg id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCatLateEarlyRuleByCfgIDList([DataSourceRequest] DataSourceRequest request, CatLateEarlyRuleByCfgIDModelSearch model)
        {
            return GetListDataAndReturn<CatLateEarlyRuleModel, Cat_LateEarlyRuleEntity, CatLateEarlyRuleByCfgIDModelSearch>(request, model, ConstantSql.hrm_cat_sp_get_LateEarlyRuleByAttGradeId);
        }


        public ActionResult GetCatLateEarlyRuleByFormulaCfgIDList([DataSourceRequest] DataSourceRequest request, CatLateEarlyRuleFormulaByCfgIDModelSearch model)
        {
            return GetListDataAndReturn<Cat_LateEarlyRuleFormulaModel, Cat_LateEarlyRuleFormulaEntity, CatLateEarlyRuleFormulaByCfgIDModelSearch>(request, model, ConstantSql.hrm_cat_sp_get_LateEarlyRuleFormulaByAttGradeId);
        }


        public ActionResult GetCatLateEarlyAttendanceByCfgIDList([DataSourceRequest] DataSourceRequest request, CatLateEarlyRuleFormulaByCfgIDModelSearch model)
        {
            return GetListDataAndReturn<Cat_LateEarlyRuleAttendanceModel, Cat_LateEarlyRuleAttendanceEntity, CatLateEarlyRuleFormulaByCfgIDModelSearch>(request, model, ConstantSql.hrm_cat_sp_get_LateEarlyAttendanceByAttGradeId);
        }

        public ActionResult DeleteInCellLateEarly([Bind(Prefix = "models")] List<CatLateEarlyRuleModel> model)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            foreach (var i in model)
            {
                var result = service.DeleteOrRemove<Cat_LateEarlyRuleEntity, CatLateEarlyRuleModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(i.ID.ToString()));
                status = result.ActionStatus;
            }

            return Json(status);
        }

        public ActionResult DeleteInCellLateEarlyFormula([Bind(Prefix = "models")] List<Cat_LateEarlyRuleFormulaModel> model)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            foreach (var i in model)
            {
                var result = service.DeleteOrRemove<Cat_LateEarlyRuleFormulaEntity, Cat_LateEarlyRuleFormulaModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(i.ID.ToString()));

            }

            return Json("");
        }

        public ActionResult DeleteInCellLateEarlyAttendance([Bind(Prefix = "models")] List<Cat_LateEarlyRuleAttendanceModel> model)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            foreach (var i in model)
            {
                var result = service.DeleteOrRemove<Cat_LateEarlyRuleAttendanceEntity, Cat_LateEarlyRuleAttendanceModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(i.ID.ToString()));

            }

            return Json("");
        }

        public ActionResult CreateInCellLateEarly([Bind(Prefix = "models")] List<CatLateEarlyRuleModel> model, Guid id)
        {
            var service = new BaseService();
            var actionSer = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            //var t = service.GetAllUserEntity<Cat_GradeMultiEntity>(ref status);
            var GradeCfg = GetData<CatLateEarlyRuleMultiModel, Cat_GradeMultiEntity>("", ConstantSql.hrm_cat_sp_get_Grade_multi);

            //get dữ liệu late eary
            var objLateEarly = Common.AddRange(3);
            objLateEarly[0] = id;
            var lstLateEarlyByGradeID = actionSer.GetData<CatLateEarlyRuleModel>(objLateEarly, ConstantSql.hrm_cat_sp_get_LateEarlyRuleByAttGradeId, ref status);


            var lstResult = new List<CatLateEarlyRuleModel>();
            if (model != null)
            {
                if (id != Guid.Empty)
                {
                    /*
                     * [Anh.Le][20/10/2016][IdTask: 74657 ][bug]
                     */
                    foreach (var item in model)
                    {
                        var lstLateEarlyByRuleType = lstLateEarlyByGradeID.Where(s => s.RuleType == item.RuleType).ToList();
                        if (lstLateEarlyByRuleType.Count > 0)
                        {
                            if (!lstLateEarlyByRuleType.Any(s => s.MinValue < item.MaxValue && s.MaxValue > item.MinValue))
                            {
                                lstResult.Add(item);
                            }
                        }
                        else
                        {
                            lstResult.Add(item);
                        }
                    }

                    if (lstResult.Count == 0)
                    {
                        return Json(NotificationType.Error.ToString());
                    }

                    #region Code Cu
                    //if (model[0].ID == Guid.Empty)
                    //{
                    //    var lateEarly = model.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()).FirstOrDefault();

                    //    if (lateEarly != null)
                    //    {
                    //        if (model.Count > 1)
                    //        {
                    //            model = model.Where(s => s.RuleType != lateEarly.RuleType).ToList();
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    var lateEarly = lstLateEarlyByGradeID.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()).FirstOrDefault();

                    //    if (lateEarly != null)
                    //    {
                    //        if (model.Count > 1)
                    //        {
                    //            model = model.Where(s => s.RuleType != lateEarly.RuleType && ( s.MinValue > lateEarly.MaxValue || lateEarly.MinValue > s.MaxValue)).ToList();
                    //        }
                    //    }
                    //}
                    #endregion
                    foreach (var i in lstResult)
                    {
                        i.GradeAttID = id;
                        if (i.ID != Guid.Empty)
                        {
                            if (lstLateEarlyByGradeID.Count > 0)
                            {
                                var lateEarlyCheck = lstLateEarlyByGradeID.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString() && i.RuleType == s.RuleType).FirstOrDefault();
                                if (lateEarlyCheck != null)
                                {
                                    /*
                                     * [23/12.2015][to.le][bug][0062213] Lối không lưu được làm tròn trễ sớm
                                     * Nếu lateEarlyCheck khác null thì sẽ lưu lại giá trị thay đổi.
                                     */
                                    service.Edit<Cat_LateEarlyRuleEntity>(i.CopyData<Cat_LateEarlyRuleEntity>());
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(i.RuleType) || i.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString())
                                    {
                                        continue;
                                    }
                                }
                            }
                            service.Edit<Cat_LateEarlyRuleEntity>(i.CopyData<Cat_LateEarlyRuleEntity>());
                        }
                        else
                        {

                            if (lstLateEarlyByGradeID.Count > 0)
                            {
                                //[To.Le][12/05/2016][bug]Nếu thêm mới 2 loại giống nhau thì xết giao lại thảo dk thì lưu lai dòng mới thêm vào.
                                var lateEarlyCheck = lstLateEarlyByGradeID.Where(s => (string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()) && i.MinValue != s.MinValue && i.MaxValue != s.MaxValue).FirstOrDefault();
                                if (lateEarlyCheck != null)
                                {
                                    service.Add<Cat_LateEarlyRuleEntity>(i.CopyData<Cat_LateEarlyRuleEntity>());
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(i.RuleType) || i.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString())
                                    {
                                        continue;
                                    }
                                }
                            }
                            service.Add<Cat_LateEarlyRuleEntity>(i.CopyData<Cat_LateEarlyRuleEntity>());

                        }
                    }
                }

            }
            return Json(NotificationType.Success.ToString());

        }



        public ActionResult CreateInCellLateEarlyFormula([Bind(Prefix = "models")] List<Cat_LateEarlyRuleFormulaModel> model, Guid id)
        {
            var service = new BaseService();
            var actionSer = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            //var t = service.GetAllUserEntity<Cat_GradeMultiEntity>(ref status);
            var GradeCfg = GetData<CatLateEarlyRuleMultiModel, Cat_GradeMultiEntity>("", ConstantSql.hrm_cat_sp_get_Grade_multi);

            //get dữ liệu late eary
            var objLateEarly = Common.AddRange(3);
            objLateEarly[0] = id;
            var lstLateEarlyByGradeID = actionSer.GetData<Cat_LateEarlyRuleFormulaModel>(objLateEarly, ConstantSql.hrm_cat_sp_get_LateEarlyRuleFormulaByAttGradeId, ref status);

            //var service = new RestServiceClient<CatLateEarlyRuleModel>();
            //service.SetCookies(this.Request.Cookies, _hrm_Cat_Service);

            if (model != null)
            {
                //   if (GradeCfg != null && GradeCfg.Count > 0 && id != Guid.Empty)
                //   {
                if (id != Guid.Empty)
                {
                    if (model[0].ID == Guid.Empty)
                    {
                        var lateEarly = model.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()).FirstOrDefault();

                        if (lateEarly != null)
                        {
                            if (model.Count > 1)
                            {
                                model = model.Where(s => s.RuleType != lateEarly.RuleType).ToList();
                            }
                        }
                    }
                    else
                    {
                        var lateEarly = lstLateEarlyByGradeID.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()).FirstOrDefault();

                        if (lateEarly != null)
                        {
                            if (model.Count > 1)
                            {
                                model = model.Where(s => s.RuleType != lateEarly.RuleType).ToList();
                            }
                        }
                    }



                    foreach (var i in model)
                    {

                        //  i.GradeCfgID = GradeCfg.FirstOrDefault().ID;
                        i.GradeAttID = id;
                        //item = service.Add<Cat_LateEarlyRuleEntity, CatLateEarlyRuleModel>(i);
                        if (i.ID != Guid.Empty)
                        {
                            if (lstLateEarlyByGradeID.Count > 0)
                            {
                                var lateEarlyCheck = lstLateEarlyByGradeID.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString() && i.RuleType == s.RuleType).FirstOrDefault();
                                if (lateEarlyCheck != null)
                                {
                                    //[23/12.2015][to.le][bug][0062213] Lối không lưu được làm tròn trễ sớm
                                    //Nếu lateEarlyCheck khác null thì sẽ lưu lại giá trị thay đổi.
                                    service.Edit<Cat_LateEarlyRuleFormulaEntity>(i.CopyData<Cat_LateEarlyRuleFormulaEntity>());
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(i.RuleType) || i.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString())
                                    {
                                        continue;
                                    }
                                }
                            }
                            service.Edit<Cat_LateEarlyRuleFormulaEntity>(i.CopyData<Cat_LateEarlyRuleFormulaEntity>());
                        }
                        else
                        {

                            if (lstLateEarlyByGradeID.Count > 0)
                            {
                                //[To.Le][12/05/2016][bug]Nếu thêm mới 2 loại giống nhau thì xết giao lại thảo dk thì lưu lai dòng mới thêm vào.
                                var lateEarlyCheck = lstLateEarlyByGradeID.Where(s => (string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()) && i.MinValue != s.MinValue && i.MaxValue != s.MaxValue).FirstOrDefault();
                                if (lateEarlyCheck != null)
                                {
                                    service.Add<Cat_LateEarlyRuleFormulaEntity>(i.CopyData<Cat_LateEarlyRuleFormulaEntity>());
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(i.RuleType) || i.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString())
                                    {
                                        continue;
                                    }
                                }
                            }
                            service.Add<Cat_LateEarlyRuleFormulaEntity>(i.CopyData<Cat_LateEarlyRuleFormulaEntity>());

                        }
                    }
                }

            }
            return Json("");

        }


        public ActionResult CreateInCellLateEarlyAttendance([Bind(Prefix = "models")] List<Cat_LateEarlyRuleAttendanceModel> model, Guid id)
        {
            var service = new BaseService();
            var actionSer = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            //var t = service.GetAllUserEntity<Cat_GradeMultiEntity>(ref status);
            var GradeCfg = GetData<CatLateEarlyRuleMultiModel, Cat_GradeMultiEntity>("", ConstantSql.hrm_cat_sp_get_Grade_multi);

            //get dữ liệu late eary
            var objLateEarly = Common.AddRange(3);
            objLateEarly[0] = id;
            var lstLateEarlyByGradeID = actionSer.GetData<Cat_LateEarlyRuleAttendanceModel>(objLateEarly, ConstantSql.hrm_cat_sp_get_LateEarlyAttendanceByAttGradeId, ref status);

            //var service = new RestServiceClient<CatLateEarlyRuleModel>();
            //service.SetCookies(this.Request.Cookies, _hrm_Cat_Service);

            if (model != null)
            {
                //   if (GradeCfg != null && GradeCfg.Count > 0 && id != Guid.Empty)
                //   {
                if (id != Guid.Empty)
                {
                    if (model[0].ID == Guid.Empty)
                    {
                        var lateEarly = model.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()).FirstOrDefault();

                        if (lateEarly != null)
                        {
                            if (model.Count > 1)
                            {
                                model = model.Where(s => s.RuleType != lateEarly.RuleType).ToList();
                            }
                        }
                    }
                    else
                    {
                        var lateEarly = lstLateEarlyByGradeID.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()).FirstOrDefault();

                        if (lateEarly != null)
                        {
                            if (model.Count > 1)
                            {
                                model = model.Where(s => s.RuleType != lateEarly.RuleType).ToList();
                            }
                        }
                    }



                    foreach (var i in model)
                    {

                        //  i.GradeCfgID = GradeCfg.FirstOrDefault().ID;
                        i.GradeAttID = id;
                        //item = service.Add<Cat_LateEarlyRuleEntity, CatLateEarlyRuleModel>(i);
                        if (i.ID != Guid.Empty)
                        {
                            if (lstLateEarlyByGradeID.Count > 0)
                            {
                                var lateEarlyCheck = lstLateEarlyByGradeID.Where(s => string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString() && i.RuleType == s.RuleType).FirstOrDefault();
                                if (lateEarlyCheck != null)
                                {
                                    //[23/12.2015][to.le][bug][0062213] Lối không lưu được làm tròn trễ sớm
                                    //Nếu lateEarlyCheck khác null thì sẽ lưu lại giá trị thay đổi.
                                    service.Edit<Cat_LateEarlyRuleAttendanceEntity>(i.CopyData<Cat_LateEarlyRuleAttendanceEntity>());
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(i.RuleType) || i.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString())
                                    {
                                        continue;
                                    }
                                }
                            }
                            service.Edit<Cat_LateEarlyRuleAttendanceEntity>(i.CopyData<Cat_LateEarlyRuleAttendanceEntity>());
                        }
                        else
                        {

                            if (lstLateEarlyByGradeID.Count > 0)
                            {
                                //[To.Le][12/05/2016][bug]Nếu thêm mới 2 loại giống nhau thì xết giao lại thảo dk thì lưu lai dòng mới thêm vào.
                                var lateEarlyCheck = lstLateEarlyByGradeID.Where(s => (string.IsNullOrEmpty(s.RuleType) || s.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString()) && i.MaxTimeAllow != s.MinMinute && i.MaxMinute != s.MaxMinute).FirstOrDefault();
                                if (lateEarlyCheck != null)
                                {
                                    service.Add<Cat_LateEarlyRuleAttendanceEntity>(i.CopyData<Cat_LateEarlyRuleAttendanceEntity>());
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(i.RuleType) || i.RuleType == EnumDropDown.LateEarlyRuleType.E_LATEEARLY.ToString())
                                    {
                                        continue;
                                    }
                                }
                            }
                            service.Add<Cat_LateEarlyRuleAttendanceEntity>(i.CopyData<Cat_LateEarlyRuleAttendanceEntity>());

                        }
                    }
                }

            }
            return Json("");

        }



        #endregion


        #region Cat_SkillCourseCertificate
        public ActionResult GetkillCourseCertificateData([DataSourceRequest] DataSourceRequest request, Cat_SkillCourseCertificateSearchModel model)
        {
            return GetListDataAndReturn<Cat_SkillCourseCertificateModel, Cat_SkillCourseCertificateEntity, Cat_SkillCourseCertificateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SkillCourseCertificate);
        }
        public ActionResult CreateInCellSkillCourseCertificate([Bind(Prefix = "models")] List<Cat_SkillCourseCertificateModel> model)
        {
            var service = new BaseService();
            var status = string.Empty;
            if (model != null)
            {
                foreach (var i in model)
                {
                    if (i.ID != Guid.Empty)
                    {
                        status = service.Edit<Cat_SkillCourseCertificateEntity>(i.CopyData<Cat_SkillCourseCertificateEntity>());
                    }
                    else
                    {
                        status = service.Add<Cat_SkillCourseCertificateEntity>(i.CopyData<Cat_SkillCourseCertificateEntity>());
                    }
                }
            }
            return Json(status);
        }

        public ActionResult DeleteInCellSkillCourseCertificate([Bind(Prefix = "models")] List<Cat_SkillCourseCertificateModel> model)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            if (model != null)
            {
                foreach (var i in model)
                {
                    var result = service.DeleteOrRemove<Cat_SkillCourseCertificateEntity, Cat_SkillCourseCertificateModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(i.ID.ToString()));
                }
            }

            return Json("");
        }

        #endregion

        #region Cat_RoundAttRule
        public ActionResult GetRoundAttRuleData([DataSourceRequest] DataSourceRequest request, Cat_RoundAttRuleModelSearch model)
        {
            return GetListDataAndReturn<Cat_RoundAttRuleModel, Cat_RoundAttRuleEntity, Cat_RoundAttRuleModelSearch>(request, model, ConstantSql.hrm_cat_sp_get_roundattrulebyatt);
        }



        public ActionResult CreateInCellRoundAttRule([Bind(Prefix = "models")] List<Cat_RoundAttRuleModel> model, Guid id)
        {
            var service = new BaseService();
            var actionSer = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            var objRoundAttRule = Common.AddRange(3);
            objRoundAttRule[0] = id;
            var lstLateEarlyByGradeID = actionSer.GetData<Cat_RoundAttRuleModel>(objRoundAttRule, ConstantSql.hrm_cat_sp_get_roundattrulebyatt, ref status);
            if (model != null)
            {
                if (id != Guid.Empty)
                {
                    foreach (var i in model)
                    {
                        i.GradeAttID = id;
                        if (i.OrderNumberView.HasValue())
                        {
                            i.OrderNumber = i.OrderNumberView.Value;
                        }
                        if (i.ID != Guid.Empty)
                        {
                            status = service.Edit<Cat_RoundAttRuleEntity>(i.CopyData<Cat_RoundAttRuleEntity>());
                        }
                        else
                        {
                            status = service.Add<Cat_RoundAttRuleEntity>(i.CopyData<Cat_RoundAttRuleEntity>());
                        }
                    }
                }
            }
            return Json(status);
        }

        public ActionResult DeleteInCellRoundAttRule([Bind(Prefix = "models")] List<Cat_RoundAttRuleModel> model)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            foreach (var i in model)
            {
                var result = service.DeleteOrRemove<Cat_RoundAttRuleEntity, Cat_RoundAttRuleModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(i.ID.ToString()));
            }
            return Json("");
        }


        public ActionResult DeleteInCellRoundAttRuleV2(Guid id)
        {
            ActionService service = new ActionService(UserLogin, LanguageCode);
            var result = service.DeleteOrRemove<Cat_RoundAttRuleEntity, Cat_RoundAttRuleModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(id.ToString()));
            return Json(result.ActionStatus);
        }

        #endregion

        #region Cat_Grade
        [HttpPost]
        public ActionResult GetGradeList([DataSourceRequest] DataSourceRequest request, Sal_GradeSearchModel model)
        {
            return GetListDataAndReturn<Sal_GradeModel, Sal_GradeEntity, Sal_GradeSearchModel>(request, model, ConstantSql.hrm_sal_sp_get_Sal_Grade);
        }

        public JsonResult GetMultiCatGrade(string text)
        {
            return GetDataForControl<CatGradeMultiModel, Cat_GradeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Grade_multi);
        }

        #endregion

        #region Cat_Supplier
        [HttpPost]
        public ActionResult GetSupplierList([DataSourceRequest] DataSourceRequest request, Cat_SupplierSearchModel model)
        {
            return GetListDataAndReturn<Cat_SupplierModel, Cat_SupplierEntity, Cat_SupplierSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Supplier);
        }

        public ActionResult ExportAllSupplierlList([DataSourceRequest] DataSourceRequest request, Cat_SupplierSearchModel model)
        {
            return ExportAllAndReturn<Cat_SupplierEntity, Cat_SupplierModel, Cat_SupplierSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Supplier);
        }

        public ActionResult ExportSupplierSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SupplierEntity, Cat_SupplierModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SupplierByIds);
        }

        public JsonResult GetMultiSupplier(string text)
        {
            return GetDataForControl<Cat_SupplierMultiModel, Cat_SupplierMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Supplier_Multi);
        }
        #endregion

        #region Cat_PurchaseItems
        public ActionResult GetPurchaseItemsList([DataSourceRequest] DataSourceRequest request, Cat_PurchaseItemsSearchModel model)
        {
            return GetListDataAndReturn<Cat_PurchaseItemsModel, Cat_PurchaseItemsEntity, Cat_PurchaseItemsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PurchaseItems);
        }

        public ActionResult ExportAllPurchaseItems([DataSourceRequest] DataSourceRequest request, Cat_PurchaseItemsSearchModel model)
        {
            return ExportAllAndReturn<Cat_PurchaseItemsEntity, Cat_PurchaseItemsModel, Cat_PurchaseItemsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PurchaseItems);
        }

        public ActionResult ExportPurchaseItemsSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PurchaseItemsEntity, Cat_PurchaseItemsModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PurchaseItemsByIds);
        }

        public JsonResult GetMultiPurchaseItems(string text)
        {
            return GetDataForControl<Cat_PurchaseItemsMultiModel, Cat_PurchaseItemsMultiEntity>(text, ConstantSql.hrm_cat_sp_get_PruchaseItems_Multi);
        }
        #endregion

        #region Cat_Overtime
        [HttpPost]
        public ActionResult GetOvertimeTypeList([DataSourceRequest] DataSourceRequest request, CatOvertimeTypeSearchModel model)
        {
            return GetListDataAndReturn<CatOvertimeTypeModel, Cat_OvertimeTypeEntity, CatOvertimeTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_OvertimeType);
        }


        public JsonResult GetMultiOvertimeType(string text)
        {
            return GetDataForControl<CatOvertimeTypeMultiModel, Cat_OvertimeTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_OvertimeType_multi);
        }
        public JsonResult GetStatusFilterApproveOT()
        {
            IList<SelectListItem> listStatus = Enum.GetValues(typeof(EnumDropDown.OverTimeStatus))
              .Cast<EnumDropDown.OverTimeStatus>()
              .Select(x => new SelectListItem { Value = x.ToString(), Text = EnumDropDown.GetEnumDescription(x.ToString().TranslateString()) })
              .ToList();
            return Json(listStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStatusFilterApproveLD()
        {
            IList<SelectListItem> listStatus = Enum.GetValues(typeof(AttendanceDataStatus))
            .Cast<AttendanceDataStatus>()
            .Select(x => new SelectListItem { Value = x.ToString(), Text = EnumDropDown.GetEnumDescription(x) })
            .ToList();
            return Json(listStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiOvertimeTypeByTimeLine(DateTime? DateEnd)
        {
            var OvertimeTypeServices = new Cat_OvertimeTypeServices();
            var Result = OvertimeTypeServices.GetOverTimeType_TimeLine(DateEnd);
            return Json(Result);
            //return GetDataForControl<CatOvertimeTypeMultiModel, Cat_OvertimeTypeMultiEntity>(null, ConstantSql.hrm_cat_sp_get_OvertimeType_multi);
        }

        public ActionResult ExportAllOvertimeTypelList([DataSourceRequest] DataSourceRequest request, CatOvertimeTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_OvertimeTypeEntity, CatOvertimeTypeModel, CatOvertimeTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_OvertimeType);
        }

        public ActionResult ExportOvertimeTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_OvertimeTypeEntity, CatOvertimeTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_OvertimeTypeByIds);
        }




        //public JsonResult GetMultiOvertimeType(string text)
        //{
        //    var service = new Cat_OvertimeTypeServices();
        //    var get = service.GetMulti(text);
        //    var result = get.Select(item => new CatOvertimeTypeMultiModel()
        //    {
        //        Id = item.Id,
        //        OvertimeTypeName = item.OvertimeTypeName,
        //    });
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //   public ActionResult GetOvertimeTypeList([DataSourceRequest] DataSourceRequest request, CatOvertimeTypeSearchModel ctModel)  
        //   {
        //    var service = new Cat_OvertimeTypeServices();
        //    ctModel.Rate = ctModel.Rate == 0.0 ? null : ctModel.Rate;
        //    ListQueryModel lstModel = new ListQueryModel
        //    {
        //        PageIndex = request.Page,
        //        Filters = ExtractFilterAttributes(request),
        //        Sorts = ExtractSortAttributes(request),
        //        AdvanceFilters = ExtractAdvanceFilterAttributes(ctModel)
        //    };
        //    var result = service.GetCat_OvertimeType(lstModel).ToList().Translate<CatOvertimeTypeModel>();
        //    return Json(result.ToDataSourceResult(request));
        //}
        #endregion

        #region Cat_HDTJobGroup
        public JsonResult GetMultiHDTJobGroup(string text)
        {
            return GetDataForControl<Cat_HDTJobGroupMultiModel, Cat_HDTJobGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_HDTJobGroup_multi);
        }

        public JsonResult GetMultiHDTJobGroupCode(string text)
        {
            return GetDataForControl<Cat_HDTJobGroupCodeMultiModel, Cat_HDTJobGroupCodeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_HDTJobGroupCode_multi);
        }
        public ActionResult ExportAllHDTJobGroupList([DataSourceRequest] DataSourceRequest request, Cat_HDTJobGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_HDTJobGroupEntity, Cat_HDTJobGroupModel, Cat_HDTJobGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_HDTJobGroup);
        }


        public ActionResult ExportHDTJobGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_HDTJobGroupEntity, Cat_HDTJobGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_HDTJobGroupByIds);
        }

        public ActionResult GetHDTJobGroupList([DataSourceRequest] DataSourceRequest request, Cat_HDTJobGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_HDTJobGroupModel, Cat_HDTJobGroupEntity, Cat_HDTJobGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_HDTJobGroup);
        }

        public ActionResult ChangeStatusListHDTJobGroup(string selectedIds, string Status)
        {
            Cat_HDTJobGroupServices Services = new Cat_HDTJobGroupServices();
            ResultsObject result = Services.UpdateStatus(selectedIds, Status);
            return Json(result);
        }
        #endregion

        #region Cat_Element
        [HttpPost]
        public ActionResult GetElementList([DataSourceRequest] DataSourceRequest request, CatElementCommissionSearchModel model)
        {
            return GetListDataAndReturn<CatElementModel, Cat_ElementEntity, CatElementCommissionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ElementByMethod);
        }
        [HttpPost]
        public ActionResult GetElementListbyMethodPayroll([DataSourceRequest] DataSourceRequest request, Guid GradeID, string MethodPayroll)
        {
            string status = string.Empty;
            List<object> lstModel = new List<object>();
            lstModel.AddRange(new object[9]);
            lstModel[2] = null;
            lstModel[3] = Common.DotNetToOracle(GradeID.ToString());
            lstModel[4] = null;
            lstModel[7] = 1;
            lstModel[8] = Int32.MaxValue - 1;
            var listEntity = baseService.GetData<CatElementModel>(lstModel, ConstantSql.hrm_cat_sp_get_Element, UserLogin, ref status).Where(m => m.MethodPayroll == MethodPayroll).ToList();
            return Json(listEntity.ToDataSourceResult(request));
        }


        public JsonResult GetHospitalMulti(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_HospitalModel>(text, ConstantSql.hrm_cat_sp_get_Hospital_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                listEntity = listEntity.OrderBy(s => s.HospitalName).ToList();
                return Json(listEntity, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetFormulaTemplateList([DataSourceRequest] DataSourceRequest request, Cat_FormulaTemplateSearchModel model)
        {
            return GetListDataAndReturn<Cat_FormulaTemplateModel, Cat_FormulaTemplateEntity, Cat_FormulaTemplateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_FormulaTemplate);
        }

        public ActionResult ExportSElementSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ElementEntity, CatElementModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ElementByIds);
        }
        public ActionResult ExportAllElementList([DataSourceRequest] DataSourceRequest request, CatElementSearchModel model)
        {
            return ExportAllAndReturn<Cat_ElementEntity, CatElementModel, CatElementSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Element);
        }

        public ActionResult GetElementListForCreate([DataSourceRequest] DataSourceRequest request, string type, Guid? GradePayroll, string TabType, string Method)
        {
            string status = string.Empty;
            List<object> lstModel = new List<object>();
            if (Method == null || Method == string.Empty)
            {
                Method = MethodPayroll.E_NORMAL.ToString();
            }

            #region Load dữ liệu của phần phụ cấp
            if (type == CatElementType.Allowances.ToString())
            {
                lstModel = new List<object>();
                lstModel.AddRange(new object[4]);
                lstModel[2] = 1;
                lstModel[3] = Int32.MaxValue - 1;
                var listUsualAllowance = baseService.GetData<Cat_UsualAllowanceModel>(lstModel, ConstantSql.hrm_cat_sp_get_UsualAllowance, UserLogin, ref status);

                List<CatElementModel> listResult = new List<CatElementModel>();
                if (listUsualAllowance != null)
                {
                    foreach (var i in listUsualAllowance)
                    {
                        listResult.Add(new CatElementModel() { ElementName = i.UsualAllowanceName, ElementCode = i.Code, Formula = i.Formula });
                    }
                }

                lstModel = new List<object>();
                lstModel.AddRange(new object[9]);
                lstModel[2] = type;
                lstModel[7] = 1;
                lstModel[8] = Int32.MaxValue - 1;
                var ListAllowances = baseService.GetData<CatElementModel>(lstModel, ConstantSql.hrm_cat_sp_get_Element, UserLogin, ref status).Where(m => m.IsApplyGradeAll != true).ToList();
                listResult.AddRange(ListAllowances);
                return Json(listResult.ToDataSourceResult(request));
            }
            #endregion

            #region Load Dữ Liệu Phụ cấp bất thường
            if (type == CatElementType.AllowancesOut.ToString())
            {
                List<Cat_UnusualAllowanceCfgEntity> listUnusualAllowanceCfg = new List<Cat_UnusualAllowanceCfgEntity>();
                lstModel = new List<object>();
                lstModel.AddRange(new object[6]);
                lstModel[4] = 1;
                lstModel[5] = Int32.MaxValue - 1;
                listUnusualAllowanceCfg = baseService.GetData<Cat_UnusualAllowanceCfgEntity>(lstModel, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status);

                List<CatElementModel> listResult = new List<CatElementModel>();
                if (listUnusualAllowanceCfg != null)
                {
                    foreach (var i in listUnusualAllowanceCfg)
                    {
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code, Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng N-1", ElementCode = i.Code + "_N_1", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng N-2", ElementCode = i.Code + "_N_2", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng N-3", ElementCode = i.Code + "_N_3", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng N-4", ElementCode = i.Code + "_N_4", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng N-5", ElementCode = i.Code + "_N_5", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng N-6", ElementCode = i.Code + "_N_6", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = "Số tiền bù " + i.UnusualAllowanceCfgName + " Tháng N-1", ElementCode = i.Code + "_AMOUNTOFOFFSET_N_1", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Tháng 3", ElementCode = i.Code + "_T3", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Theo Timeline", ElementCode = i.Code + "_TIMELINE", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Theo Timeline Tháng N-1", ElementCode = i.Code + "_TIMELINE_N_1", Formula = i.Code });

                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Theo Kỳ Chốt Lương", ElementCode = i.Code + "_DAYCLOSE", Formula = i.Code });
                        listResult.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName + " Theo Kỳ Chốt Lương", ElementCode = i.Code + "_DAYCLOSE_N_1", Formula = i.Code });

                    }
                }
                lstModel = new List<object>();
                lstModel.AddRange(new object[9]);
                lstModel[2] = type;
                lstModel[7] = 1;
                lstModel[8] = Int32.MaxValue - 1;
                var ListUnusualAllowanceCfg = baseService.GetData<CatElementModel>(lstModel, ConstantSql.hrm_cat_sp_get_Element, UserLogin, ref status).Where(m => m.IsApplyGradeAll != true).ToList();
                listResult.AddRange(ListUnusualAllowanceCfg);
                return Json(listResult.ToDataSourceResult(request));
            }
            #endregion

            #region Ứng lương
            if (type == CatElementType.AdvancePayment.ToString())
            {
                List<CatElementModel> listAdvancePayment = new List<CatElementModel>();
                return Json(listAdvancePayment.ToDataSourceResult(request));
            }
            #endregion

            #region Hold Salary

            if (type == CatElementType.HoldSalaryItem.ToString() || type == CatElementType.ExChangeRate.ToString() || type == CatElementType.Evaluation.ToString() || type == CatElementType.FLIGHT.ToString() || type == CatElementType.Attendance.ToString())
            {
                lstModel = new List<object>();
                lstModel.AddRange(new object[9]);
                lstModel[2] = type;
                lstModel[7] = 1;
                lstModel[8] = Int32.MaxValue - 1;
                var listAtt = baseService.GetData<CatElementModel>(lstModel, ConstantSql.hrm_cat_sp_get_Element, UserLogin, ref status).Where(m => m.IsApplyGradeAll != true);
                return Json(listAtt.ToDataSourceResult(request));
            }

            #endregion



            lstModel = new List<object>();
            lstModel.AddRange(new object[9]);
            lstModel[2] = type;
            lstModel[3] = GradePayroll;
            lstModel[4] = TabType;
            lstModel[7] = 1;
            lstModel[8] = Int32.MaxValue - 1;
            var listEntity = baseService.GetData<CatElementModel>(lstModel, ConstantSql.hrm_cat_sp_get_Element, UserLogin, ref status).Where(m => m.MethodPayroll == Method).ToList();
            return Json(listEntity.ToDataSourceResult(request));
        }

        public ActionResult GetFormulaTemplate([DataSourceRequest] DataSourceRequest request, string Type, Guid GradeID)
        {
            string status = string.Empty;
            List<object> listModel = new List<object>();
            listModel.AddRange(new object[7]);
            listModel[5] = 1;
            listModel[6] = Int32.MaxValue - 1;
            List<Cat_FormulaTemplateEntity> listCat_FormulaTemplate = baseService.GetData<Cat_FormulaTemplateEntity>(listModel, ConstantSql.hrm_cat_sp_get_FormulaTemplate, UserLogin, ref status).Where(m => m.Type == Type && m.GradeID == GradeID).ToList();

            string RootPath = Common.GetPath("FormulaTemplate\\TemplateFormula.xml");
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;
            FileStream fs = new FileStream(RootPath, FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName("FormulaTemplateNode");

            foreach (XmlNode i in xmlnode)
            {
                Cat_FormulaTemplateEntity item = new Cat_FormulaTemplateEntity();
                item.ElementCode = i.Attributes["ElementName"].Value;
                item.ElementName = i.Attributes["ElementCode"].Value;
                item.Invisible = false;
                item.IsBold = false;
                item.Type = i.Attributes["Type"].Value;
                item.Formula = i.Attributes["Formula"].Value;

                if (!listCat_FormulaTemplate.Any(m => m.ElementCode == item.ElementCode))
                {
                    listCat_FormulaTemplate.Add(item);
                }
            }
            return Json(listCat_FormulaTemplate.Where(m => m.Type == Type).ToDataSourceResult(request));
        }

        /// <summary>
        /// Lấy dữ liệu Enum cho lưới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetElementInEnum([DataSourceRequest] DataSourceRequest request)
        {
            IList<SelectListItem>
             valuesAsList = Enum.GetValues(typeof(PayrollElement))
             .Cast<PayrollElement>
                 ()
                 .Select(x => new SelectListItem { Value = x.ToString(), Text = EnumDropDown.GetEnumDescription(x) })
                 .ToList();
            List<CatElementModel> listModel = new List<CatElementModel>();
            foreach (var i in valuesAsList)
            {
                listModel.Add(new CatElementModel() { ElementCode = i.Value.ToString(), ElementName = i.Text.ToString(), Formula = "[" + i.Value.ToString() + "]" });
            }
            return Json(listModel.ToDataSourceResult(request));
        }

        public ActionResult GetElementInEnumGroup([DataSourceRequest] DataSourceRequest request)
        {
            IList<SelectListItem>
             valuesAsList = Enum.GetValues(typeof(PayrollElementGroup))
             .Cast<PayrollElementGroup>
                 ()
                 .Select(x => new SelectListItem { Value = x.ToString(), Text = EnumDropDown.GetEnumDescription(x) })
                 .ToList();
            List<CatElementModel> listModel = new List<CatElementModel>();
            foreach (var i in valuesAsList)
            {
                listModel.Add(new CatElementModel() { ElementCode = i.Value.ToString(), ElementName = i.Text.ToString(), Formula = "[" + i.Value.ToString() + "]" });
            }
            return Json(listModel.ToDataSourceResult(request));
        }

        public ActionResult GetInsuranceElementInEnum([DataSourceRequest] DataSourceRequest request, string category)
        {
            var valuesAsList = Enum.GetValues(typeof(InsuranceElement))
                .Cast<InsuranceElement>().Select(x =>
                new
                {
                    Value = x.ToString(),
                    //mo ta phan tu
                    Text = EnumDropDown.GetEnumDescription(x),
                    //loai enum ([INS_SALARY], [INS_14DAYS], [INS_JOBNAME])
                    Category = EnumDropDown.GetEnumCategory(x)
                }).ToList();
            if (!string.IsNullOrEmpty(category))
            {
                valuesAsList = valuesAsList.Where(x => x.Category == category).ToList();
            }
            List<CatElementModel> listModel = new List<CatElementModel>();
            foreach (var i in valuesAsList)
            {
                listModel.Add(new CatElementModel() { ElementCode = i.Value.ToString(), ElementName = i.Text.ToString(), ElementType = i.Category.ToString(), Formula = "[" + i.Value.ToString() + "]", TypeElement = ConstantDisplay.HRM_Category_TypeElementStatic.TranslateString(LanguageCode), });
            }

            if (category == ConstantInsuranceElement.INS_SALARY.ToString())
            {
                #region Lấy danh sách phụ cấp động thêm vào list phần tử
                //[Tung.LY 20160217][63329]: Lấy enum các loại phụ cấp theo lương cơ bản, lấy động theo mã phụ cấp.(Tên enum = Cat_UsualAllowance.Code)
                string status = string.Empty;
                var baseService = new BaseService();
                var listModelUsualAllowance = new List<object>();
                listModelUsualAllowance.AddRange(new object[4]);
                listModelUsualAllowance[2] = 1;
                listModelUsualAllowance[3] = Int32.MaxValue - 1;
                var usualAllowanceResult = baseService.GetData<Cat_UsualAllowanceEntity>(listModelUsualAllowance, ConstantSql.hrm_cat_sp_get_UsualAllowance, UserLogin, ref status);

                #region [Tung.Ly 20170828][87242]: lay danh sach mã phu cap có số tiền lớn nhất (Sal_Basicsalary.E_AllowanceAmount1....)
                var usualAllowanceResultMax = usualAllowanceResult.Translate<Cat_UsualAllowanceEntity>();
                foreach (var allowanceItem in usualAllowanceResultMax)
                {
                    allowanceItem.Code = "MAX_" + allowanceItem.Code;
                }
                usualAllowanceResult.AddRange(usualAllowanceResultMax);
                #endregion

                if (usualAllowanceResult != null && usualAllowanceResult.Any())
                {
                    usualAllowanceResult = usualAllowanceResult.Where(m => m.Code != null).ToList();
                    foreach (var item in usualAllowanceResult)
                    {
                        // Nếu công thức có chứa khoảng trắng => không hiển thị lên.
                        if (item.Code.Contains(" "))
                        {
                            continue;
                        }
                        listModel.Add(new CatElementModel() { ElementCode = item.Code, ElementType = "INS_SALARY", ElementName = item.UsualAllowanceName, Formula = "[" + item.Code + "]", TypeElement = ConstantDisplay.HRM_Category_TypeElementDynamic.TranslateString(LanguageCode), });
                    }
                }
                #endregion
            }

            return Json(listModel.ToDataSourceResult(request));
        }

        /// <summary>
        /// [Hien.Nguyen] Tạo ra các phần tử tính lương theo mã loại ngày nghỉ và tăng ca
        /// Mỗi loại tăng ca và mỗi lại ngày nghỉ đều lưu lại là phần tử
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateFirstElement()
        {
            ActionService action = new ActionService(UserLogin);
            string status = string.Empty;
            List<object> listobject = new List<object>();
            listobject = new List<object>();
            listobject = Common.AddRange(9);
            var listElement = baseService.GetData<Cat_ElementEntity>(listobject, ConstantSql.hrm_cat_sp_get_Element, UserLogin, ref status);

            List<Cat_ElementEntity> listModel = new List<Cat_ElementEntity>();

            #region Xóa hết các phần tử
            //Chỉ xóa các phần tử nào được hệ thống tạo ra (phần tử có mã và công thức = nhau)
            //List<Cat_ElementEntity> _temp = listElement.Where(m => m.ElementType == CatElementType.Comission.ToString() || m.ElementType == CatElementType.Evaluation.ToString() || m.ElementType == CatElementType.Attendance.ToString() || m.ElementType == CatElementType.FLIGHT.ToString() || m.ElementType == CatElementType.HoldSalaryItem.ToString() || m.ElementType == CatElementType.AllowancesOut.ToString() || m.ElementType == CatElementType.Allowances.ToString() || m.ElementType == CatElementType.ExChangeRate.ToString()).ToList();
            List<Cat_ElementEntity> _temp = listElement.Where(m => "[" + m.ElementCode + "]" == m.Formula).ToList();

            if (_temp != null && _temp.Count > 0)
            {
                Cat_ElementServices services = new Cat_ElementServices();
                services.DeleteElement(_temp.Select(m => m.ID).ToList());

            }
            #endregion
            Cat_ElementServices elementServices = new Cat_ElementServices();
            listModel = elementServices.GetFirstElement(UserLogin);
            baseService.Add<Cat_ElementEntity>(listModel.ToArray());
            return Json(true);
        }
        #endregion


        #region Cat_AdvancePayment
        public ActionResult GetAdvancePaymentList([DataSourceRequest] DataSourceRequest request, Cat_AdvancePaymentSearchModel model)
        {
            return GetListDataAndReturn<CatElementModel, Cat_ElementEntity, Cat_AdvancePaymentSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AdvancePayment);
        }
        #endregion

        #region Cat_EthnicGroup

        [HttpPost]
        public ActionResult GetEthnicGroupList([DataSourceRequest] DataSourceRequest request, CatEthnicGroupSearchModel model)
        {
            return GetListDataAndReturn<CatEthnicGroupModel, Cat_EthnicGroupEntity, CatEthnicGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EthnicGroup);
        }

        public ActionResult ExportEthnicGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_EthnicGroupEntity, CatEthnicGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_EthnicGroupsByIds);
        }

        public ActionResult ExportAllEthnicGroup([DataSourceRequest] DataSourceRequest request, CatEthnicGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_EthnicGroupEntity, CatEthnicGroupModel, CatEthnicGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EthnicGroup);
        }

        /// <summary>
        /// Lấy danh sách chức vụ
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEthnicGroup()
        {
            //var service = new Cat_EthnicGroupServices();
            var result = baseService.GetAllUseEntity<Cat_EthnicGroupEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEthnicGroupOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_EthnicGroupMultiModel>(text, ConstantSql.hrm_cat_sp_get_EthnicGroup_Multi, UserLogin, ref status);
            if (listEntity != null)
            {

                listEntity = listEntity.OrderBy(s => s.EthnicGroupName).ToList();
                return Json(listEntity, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiEthnicGroup(string text)
        {
            return GetDataForControl<Cat_EthnicGroupMultiModel, Cat_EthnicGroupMultiModel>(text, ConstantSql.hrm_cat_sp_get_EthnicGroup_Multi);
        }
        #endregion

        #region Cat_ReqDocument

        [HttpPost]
        public ActionResult GetReqDocumentList([DataSourceRequest] DataSourceRequest request, Cat_ReqDocumentSearchModel model)
        {
            return GetListDataAndReturn<Cat_ReqDocumentModel, Cat_ReqDocumentEntity, Cat_ReqDocumentSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ReqDocument);
        }

        public ActionResult ExportCatReqDocumentSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ReqDocumentEntity, Cat_ReqDocumentModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ReqDocumentByIds);
        }

        public ActionResult ExportAllReqDocumentList([DataSourceRequest] DataSourceRequest request, Cat_ReqDocumentSearchModel model)
        {
            return ExportAllAndReturn<Cat_ReqDocumentEntity, Cat_ReqDocumentModel, Cat_ReqDocumentSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ReqDocument);

        }

        public JsonResult GetMultiReqDocument(string text)
        {
            return GetDataForControl<Cat_ReqDocumentModel, Cat_ReqDocumentEntity>(text, ConstantSql.hrm_hr_sp_get_ReqDocument_multi);
        }

        //Son.Vo - 20160519 - 0067779
        public JsonResult GetMultiReqDocumentQuit(string text)
        {
            return GetDataForControl<Cat_ReqDocumentModel, Cat_ReqDocumentEntity>(text, ConstantSql.hrm_cat_sp_get_ReqDocumentQuit_multi);
        }

        //Son.Vo - 20160519 - 0067778
        public JsonResult GetMultiReqDocumentRelative(string text)
        {
            return GetDataForControl<Cat_ReqDocumentModel, Cat_ReqDocumentEntity>(text, ConstantSql.hrm_cat_sp_get_ReqDocumentRelative_multi);
        }

        #endregion

        #region Cat_ExportItem

        //[HttpPost]
        public ActionResult GetExportItemList([DataSourceRequest] DataSourceRequest request, CatExportItemModel model)
        {
            return GetListDataAndReturn<CatExportItemModel, Cat_ExportItemEntity, CatExportItemModel>(request, model, ConstantSql.hrm_cat_sp_get_ExportItem);
        }

        /// <summary>
        /// Xóa cột trên lưới cấu hình export
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteColumnExport(Guid ExportID, String DeleteColumnName)
        {
            var rpExportItemService = new Cat_ExportItemServices();
            if (string.IsNullOrEmpty(DeleteColumnName) || ExportID == Guid.Empty)
            {
                return Json(string.Empty);
            }

            rpExportItemService.DeleteColumn(DeleteColumnName, ExportID);
            return Json(string.Empty);
        }

        /// <summary>
        /// Chèn thêm cột trên lưới cấu hình export
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertColumnExport(Guid ExportID, String ColumnStartChange, int LenghtInsert)
        {
            var rpExportItemService = new Cat_ExportItemServices();
            if (string.IsNullOrEmpty(ColumnStartChange) || LenghtInsert <= 0 || ExportID == Guid.Empty)
            {
                return Json(string.Empty);
            }

            rpExportItemService.InsertColumn(ColumnStartChange, LenghtInsert, ExportID);
            return Json(string.Empty);
        }

        //[HttpPost]
        public ActionResult GetExportItemByExportIDList([DataSourceRequest] DataSourceRequest request, Guid ExportID)
        {

            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ExportID);
            var result = baseService.GetData<Cat_ExportItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ExportItemByExportID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));


            //string status = string.Empty;
            //var baseService = new BaseService();
            //var objs = new List<object>();
            //objs.Add(ExportID);

            //var service = new Cat_ExportItemServices();
            //var result = baseService.GetData<Cat_ExportItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ExportItemByExportID, ref status);
            //return Json(result.ToDataSourceResult(request));
        }

        public ActionResult GetPivotItemByPivotIDList([DataSourceRequest] DataSourceRequest request, Guid PivotID)
        {

            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(PivotID);
            var result = baseService.GetData<Cat_PivotItemEntity>(objs, ConstantSql.hrm_cat_sp_get_PivotItemByPivotID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));


            //string status = string.Empty;
            //var baseService = new BaseService();
            //var objs = new List<object>();
            //objs.Add(ExportID);

            //var service = new Cat_ExportItemServices();
            //var result = baseService.GetData<Cat_ExportItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ExportItemByExportID, ref status);
            //return Json(result.ToDataSourceResult(request));
        }


        #endregion

        #region Cat_Export

        [HttpPost]
        public ActionResult GetExportList([DataSourceRequest] DataSourceRequest request, CatExportSearchModel model)
        {
            return GetListDataAndReturn<CatExportModel, Cat_ExportEntity, CatExportSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Export);

        }
        [HttpPost]
        public ActionResult GetPivotList([DataSourceRequest] DataSourceRequest request, Cat_PivotSearchModel model)
        {
            return GetListDataAndReturn<Cat_PivotModel, Cat_PivotEntity, Cat_PivotSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Pivot);
        }

        public JsonResult GetMultiExport(string text)
        {
            return GetDataForControl<CatExportMultiModel, Cat_ExportMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Export_multi);
        }
        public JsonResult GetMultiExportExcel(string text)
        {
            return GetDataForControl<CatExportMultiModel, Cat_ExportMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ExportExcel_multi);
        }

        public JsonResult GetMultiExportWord(string text)
        {
            return GetDataForControl<CatExportMultiModel, Cat_ExportMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ExportWord_multi);
        }

        public JsonResult GetMultiReportMapping(string text)
        {
            return GetDataForControl<CatExportMultiModel, Cat_ExportMultiEntity>(text, ConstantSql.hrm_cat_sp_get_reportMapping_multi);
        }

        [HttpPost]
        public ActionResult GetExportWordList([DataSourceRequest] DataSourceRequest request, CatExportSearchModel model)
        {
            return GetListDataAndReturn<CatExportModel, Cat_ExportEntity, CatExportSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ExportWord);
        }
        #endregion

        #region Cat_GradeAttendance

        [HttpPost]
        public ActionResult GetGradeAttendanceList([DataSourceRequest] DataSourceRequest request, Cat_GradeAttendanceSearchModel model)
        {
            return GetListDataAndReturn<Cat_GradeAttendanceModel, Cat_GradeAttendanceEntity, Cat_GradeAttendanceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_GradeAttendance);
        }

        public JsonResult GetMultiGradeAttendance(string text)
        {
            return GetDataForControl<Cat_GradeAttendanceMultiModel, Cat_GradeAttendanceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_GradeAttendance_Multi);
        }


        public ActionResult ExportAllGradeAttendancelList([DataSourceRequest] DataSourceRequest request, Cat_GradeAttendanceSearchModel model)
        {
            return ExportAllAndReturn<Cat_GradeAttendanceEntity, Cat_GradeAttendanceModel, Cat_GradeAttendanceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_GradeAttendance);
        }

        public ActionResult ExportGradeAttendanceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_GradeAttendanceEntity, Cat_GradeAttendanceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_Cat_GradeAttendanceByIds);
        }

        // Son.Vo - 20160629 -  theo task 0052657
        [HttpPost]
        public ActionResult GetGradeAttendanceByRank(string Rank)
        {
            var _GradeAttendanceServices = new Cat_GradeAttendanceServices();
            var result = _GradeAttendanceServices.GetListGradeAttendanceByRank(Rank);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Son.Vo - 20160629 - theo task 0052657
        [HttpPost]
        public ActionResult GetGradePayrollByRank(string Rank)
        {
            var _GradeAttendanceServices = new Cat_GradeAttendanceServices();
            var result = _GradeAttendanceServices.GetListGradePayrollByRank(Rank);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_ImportItem
        public ActionResult GetAllCatImportItem([DataSourceRequest] DataSourceRequest request, CatImportItemModel model)
        {
            return GetListDataAndReturn<CatImportItemModel, Cat_ImportItemEntity, CatImportItemModel>(request, model, ConstantSql.hrm_cat_sp_get_ImportItem);
        }

        public JsonResult GetDataOfObjectName(string text, string objectNameRoot, string objectName)
        {
            string status = string.Empty;
            var modules = typeof(HRM.Infrastructure.Utilities.ModuleKey).GetEnumNames();
            if (objectName.Length > 3 && modules.Contains(objectName.Substring(0, 3)))
            {
                var service = new Cat_BankServices();
                var get = service.GetDataOfObjectName(objectName);
                var result = get.Select(item => new CatChildFieldsMultiModel()
                {
                    ID = item,
                    Name = item
                });
                result = result.OrderBy(p => p.Name);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetMultiChildField(string text, string objectNameRoot, string objectName)
        {
            if (!string.IsNullOrEmpty(objectName))
            {
                if (objectName.Contains("("))
                {// Bỏ phần (), chỉ lấy tên objectName để ChildFieldValue2 get được dữ liệu
                    objectName = objectName.Substring(0, objectName.LastIndexOf("(") - 1);
                }
            }

            //if (!string.IsNullOrEmpty(objectName) && objectName.LastIndexOf("2") == objectName.Length - 1)
            //{
            //   // objectName = objectName.Substring(0, objectName.Length - 1);
            //}

            string status = string.Empty;
            var modules = typeof(HRM.Infrastructure.Utilities.ModuleKey).GetEnumNames();
            if (objectName.Length > 3 && modules.Contains(objectName.Substring(0, 3)))
            {
                var service = new Cat_ImportItemServices();
                if (objectName.Contains("."))
                {
                    objectName = objectName.Substring(objectName.IndexOf('.') + 1, objectName.Length - objectName.IndexOf('.') - 1);
                }
                #region Code cu~ (su? du?ng store)
                //List<object> objs = new List<object>();
                //objs.Add(text ?? string.Empty);
                //objs.Add(objectName);
                //objs.Add(1);
                //objs.Add(100);
                //    var get1 = service.GetData<CatChildFieldsMultiModel>(objs, ConstantSql.hrm_cat_sp_get_Import_childfield_multi, ref status);
                //var get = service.GetChildFiledMulti(text, objectName); 
                #endregion

                var get = service.GetEntityProperties(objectNameRoot, objectName).Select(p => new
                {
                    ID = p[0],
                    Name = p[1]
                }).ToList();

                var result = get.Select(item => new CatChildFieldsMultiModel()
                {
                    ID = item.ID,
                    Name = item.Name
                }).ToList();
                if (objectNameRoot == "Hre_Profile")
                {
                    var moreInfo = service.GetEntityProperties("Hre_ProfileMoreInfo", "Hre_ProfileMoreInfo").Select(item => new CatChildFieldsMultiModel()
                    {
                        ID = item[0],
                        Name = "Hre_ProfileMoreInfo." + item[1]
                    }).ToList();
                    result.AddRange(moreInfo);
                }
                result = result.OrderBy(p => p.Name).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetMultiChildNameField(string text, string objectNameRoot, string objectName)
        {
            if (!string.IsNullOrEmpty(objectName))
            {
                var result = new List<CatChildFieldsMultiModel>();
                var CatChildFieldsMultiModel = new CatChildFieldsMultiModel();
                var tablename = objectName.Split('_');
                if (objectName == "Cat_AbilityTile")
                {
                    CatChildFieldsMultiModel.Name = "AbilityTitleVNI";

                }
                else
                {
                    CatChildFieldsMultiModel.Name = tablename[1] + "Name";
                }
                result.Add(CatChildFieldsMultiModel);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Cat_AppendixContractType

        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng loại phụ lục hợp đồng (Cat_AppendixContractType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAppendixContractTypeList([DataSourceRequest] DataSourceRequest request, Cat_AppendixContractTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_AppendixContractTypeModel, Cat_AppendixContractTypeEntity, Cat_AppendixContractTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AppendixContractType);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho loại phụ lục hợp đồng (Cat_AppendixContractType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllAppendixContractTypeList([DataSourceRequest] DataSourceRequest request, Cat_AppendixContractTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_AppendixContractTypeEntity, Cat_AppendixContractTypeModel, Cat_AppendixContractTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AppendixContractType);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của loại phụ lục hợp đồng (Cat_AppendixContractType) theo điều kiện tìm kiếm
        public ActionResult ExportAppendixContractTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_AppendixContractTypeEntity, Cat_AppendixContractTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_AppendixContractTypeByIds);
        }

        #endregion

        #region Cat_RewardedType

        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng loại Khen Thưởng (Cat_RewardedType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRewardedTypeList([DataSourceRequest] DataSourceRequest request, Cat_RewardedTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_RewardedTypeModel, Cat_RewardedTypeEntity, Cat_RewardedTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedType);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho loại Khen Thưởng (Cat_RewardedType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllRewardedTypeList([DataSourceRequest] DataSourceRequest request, Cat_RewardedTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_RewardedTypeEntity, Cat_RewardedTypeModel, Cat_RewardedTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedType);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn củaloại Khen Thưởng (Cat_RewardedType) theo điều kiện tìm kiếm
        public ActionResult ExportRewardedTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RewardedTypeEntity, Cat_RewardedTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RewardedTypeByIds);
        }

        public JsonResult GetMultiRewardedType(string text)
        {
            return GetDataForControl<Cat_RewardedTypeMultiModel, Cat_RewardedTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_RewardType_multi);
        }

        #endregion

        #region Cat_SalaryClassType
        public JsonResult GetMultiSalaryClassType(string text)
        {
            return GetDataForControl<Cat_SalaryClassTypeMultiModel, Cat_SalaryClassTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalaryClassType_multi);
        }
        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng loại mã lương (Cat_SalaryClassType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSalaryClassTypeList([DataSourceRequest] DataSourceRequest request, Cat_SalaryClassTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_SalaryClassTypeModel, Cat_SalaryClassTypeEntity, Cat_SalaryClassTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryClassType);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho loại mã lương (Cat_SalaryClassType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllSalaryClassTypeList([DataSourceRequest] DataSourceRequest request, Cat_SalaryClassTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_SalaryClassTypeEntity, Cat_SalaryClassTypeModel, Cat_SalaryClassTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryClassType);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của loại mã lương (Cat_SalaryClassType) theo điều kiện tìm kiếm
        public ActionResult ExportSalaryClassTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SalaryClassTypeEntity, Cat_SalaryClassTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassTypeByIds);
        }

        #endregion

        #region Cat_AccidentType
        //public JsonResult GetMultiAccidentType(string text)
        //{
        //    return GetDataForControl<Cat_AccidentTypeMultiModel, Cat_AccidentTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_AccidentType_Multi);
        //}
        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng loại tai nạn lao động (Cat_AccidentType)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAccidentTypeList([DataSourceRequest] DataSourceRequest request, Cat_AccidentTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_AccidentTypeModel, Cat_AccidentTypeEntity, Cat_AccidentTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AccidentType);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho loại tai nạn lao động (Cat_AccidentType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllAccidentTypeList([DataSourceRequest] DataSourceRequest request, Cat_AccidentTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_AccidentTypeEntity, Cat_AccidentTypeModel, Cat_AccidentTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AccidentType);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của loại tai nạn lao động (Cat_AccidentType) theo điều kiện tìm kiếm
        public ActionResult ExportAccidentTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_AccidentTypeEntity, Cat_AccidentTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_AccidentTypeByIds);
        }

        #endregion

        #region Cat_Qualification

        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Trình Độ Chuyên Môn (Cat_Qualification)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetQualificationList([DataSourceRequest] DataSourceRequest request, Cat_QualificationSearchModel model)
        {
            return GetListDataAndReturn<CatQualificationModel, Cat_QualificationEntity, Cat_QualificationSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Qualification);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Trình Độ Chuyên Môn (Cat_Qualification) theo điều kiện tìm kiếm
        //   [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllQualificationList([DataSourceRequest] DataSourceRequest request, Cat_QualificationSearchModel model)
        {
            return ExportAllAndReturn<Cat_QualificationEntity, CatQualificationModel, Cat_QualificationSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Qualification);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của Trình Độ Chuyên Môn (Cat_Qualification) theo điều kiện tìm kiếm
        public ActionResult ExportQualificationSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_QualificationEntity, CatQualificationModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_QualificationByIds);
        }
        #endregion

        #region Cat_QualificationLevel
        [HttpPost]
        /// <summary>
        /// [Phuc.Nguyen] - Lấy danh sách dữ liệu (Cat_NameEntity)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetQualificationLevelList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }

        #endregion

        #region Cat_EducationLevel
        public JsonResult GetMultiCatNameEntityByType(string Type)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(Type, ConstantSql.hrm_cat_sp_get_NameEntityByType_multi);
        }

        public JsonResult GetMultiCutOffDurationType(string Type)
        {
            return GetDataForControl<Cat_CutOffDurationTypeMultiModel, Cat_CutOffDurationTypeMultiEntity>(Type, ConstantSql.hrm_cat_sp_get_CutOffDurationType_Multi);
        }

        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Trình Độ Học Vấn (Cat_NameEntity)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEducationLevelList([DataSourceRequest] DataSourceRequest request, Cat_EducationLevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_EducationLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EducationLevel);
        }

        /// <summary>Danh Sách công thức</summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEvaluationFormularList([DataSourceRequest] DataSourceRequest request, Cat_EducationLevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_EducationLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EvaluationFormular);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllEvaluationFormularList([DataSourceRequest] DataSourceRequest request, Cat_EducationLevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_EducationLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EvaluationFormular);
        }

        public ActionResult ExportEvaluationFormularSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_EvaluationFormularByIds);
        }

        /// <summary>
        /// [Tung.Ly] - Lấy danh sách dữ liệu
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInsuranceTypeList([DataSourceRequest] DataSourceRequest request, Cat_InsuranceTypeSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_InsuranceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_InsuranceType);
        }



        [HttpPost]
        public ActionResult GetDisciplineReasonList([DataSourceRequest] DataSourceRequest request, Cat_DisciplineReasonSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_DisciplineReasonSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DisciplineReason);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllDisciplineReasonList([DataSourceRequest] DataSourceRequest request, Cat_DisciplineReasonSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_DisciplineReasonSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_DisciplineReason);
        }


        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Trình Độ Học Vấn (Cat_NameEntity) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllEducationLevelList([DataSourceRequest] DataSourceRequest request, Cat_EducationLevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_EducationLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EducationLevel);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllInsuranceTypeList([DataSourceRequest] DataSourceRequest request, Cat_InsuranceTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_InsuranceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_InsuranceType);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của Trình Độ Học Vấn (Cat_NameEntity) theo điều kiện tìm kiếm
        public ActionResult ExportEducationLevelSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_EducationLevelByIds);
        }

        public ActionResult ExportInsuranceTypeSelected(string selectedIds, [DataSourceRequest] DataSourceRequest request, Cat_InsuranceTypeSearchModel model)
        {
            // Dùng ExportSelectedAndReturn không dịch được enum EnumTypeView 
            //return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_InsuranceTypeByIds);

            List<Guid> lstselectedIds = new List<Guid>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                lstselectedIds = Common.StringToList(selectedIds);
            }
            model.SetPropertyValue("IsExport", true);
            string fullPath = string.Empty, status = string.Empty;
            var listModel = GetListData<CatNameEntityModel, Cat_NameEntityEntity, Cat_InsuranceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_InsuranceType, ref status);
            if (listModel.Any())
            {
                listModel = listModel.Where(x => lstselectedIds.Contains(x.ID)).ToList();
            }
            if (status == NotificationType.Success.ToString())
            {
                status = ExportService.ExportAll(listModel, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));
            }
            return Json(status);
        }

        public ActionResult GetMutilEducationLevel(string selectedIds)
        {
            var service = new BaseService();
            string status = "";
            var result = service.GetData<Cat_NameEntityEntity>(selectedIds, ConstantSql.hrm_cat_sp_get_EducationLevelByIds, UserLogin, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiDisciplineReason(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Discipline_Multi);
        }

        #endregion

        #region Cat_GraduatedLevel

        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Trình Độ văn hóa (Cat_NameEntity)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetGraduatedLevelList([DataSourceRequest] DataSourceRequest request, Cat_GraduatedLevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_GraduatedLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GraduatedLevel);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Trình Độ Học Vấn (Cat_NameEntity) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllGraduatedLevelList([DataSourceRequest] DataSourceRequest request, Cat_GraduatedLevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_GraduatedLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GraduatedLevel);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của Trình Độ Học Vấn (Cat_NameEntity) theo điều kiện tìm kiếm
        public ActionResult ExportGraduatedLevelSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_GraduatedLevelByIds);
        }

        //Son.Vo - 20160826 - 0072218
        public JsonResult GetMultiGraduatedLevel(string text)
        {
            string status = string.Empty;
            var services = new ActionService(LanguageCode);
            var obj = new List<object>();
            obj.AddRange(new object[3]);
            obj[0] = text;
            obj[1] = 1;
            obj[2] = int.MaxValue - 1;
            var lstGraduatedLevel = baseService.GetData<Cat_GraduatedLevelMultiEntity>(obj, ConstantSql.hrm_cat_sp_get_GraduatedLevel, UserLogin, ref status);
            if (lstGraduatedLevel != null)
            {
                lstGraduatedLevel = lstGraduatedLevel.OrderBy(s => s.Order).ThenBy(s => s.NameEntityName).ToList();
            }
            return Json(lstGraduatedLevel, JsonRequestBehavior.AllowGet);
        }

        #region Cat_CompetenceGroup
        [HttpPost]
        public ActionResult GetCompetenceGroupList([DataSourceRequest] DataSourceRequest request, Cat_CompetenceGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_CompetenceGroupModel, Cat_CompetenceGroupEntity, Cat_CompetenceGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CompetenceGroup);
        }
        public JsonResult GetMultiCompetenceGroup(string text)
        {
            return GetDataForControl<Cat_CompetenceGroupModel, Cat_CompetenceGroupEntity>(text, ConstantSql.hrm_cat_sp_get_CompetenceGroup_Multi);
        }

        public ActionResult ExportCompetenceGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CompetenceGroupEntity, Cat_CompetenceGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CompetenceGroupByIds);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllCompetenceGroupList([DataSourceRequest] DataSourceRequest request, Cat_CompetenceGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_CompetenceGroupEntity, Cat_CompetenceGroupModel, Cat_CompetenceGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CompetenceGroup);
        }

        #endregion

        [HttpPost]
        public ActionResult GetPriorityList([DataSourceRequest] DataSourceRequest request, Cat_PrioritySearchModel model)
        {
            return GetListDataAndReturn<Cat_PriorityModel, Cat_PriorityEntity, Cat_PrioritySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Priority);
        }

        public JsonResult GetMultiPriority(string text)
        {
            return GetDataForControl<Cat_PriorityMultiModel, Cat_PriorityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Priority_Multi);
        }
        public ActionResult ExportPrioritySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PriorityEntity, Cat_PriorityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PriorityByIds);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllPriorityList([DataSourceRequest] DataSourceRequest request, Cat_PrioritySearchModel model)
        {
            return ExportAllAndReturn<Cat_PriorityEntity, Cat_PriorityModel, Cat_PrioritySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Priority);
        }
        public JsonResult GetMultiNationalityGroup(string text)
        {
            return GetDataForControl<CatNationalityGroupMultiModel, Cat_NationalityGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_NationalityGroup_Multi);
        }

        public JsonResult GetMultiReasonChangeSalary(string text)
        {
            return GetDataForControl<CatNationalityGroupMultiModel, Cat_NationalityGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ReasonChangeSalary_Multi);
        }

        public JsonResult GetMultiEmployeeGroup(string text)
        {
            return GetDataForControl<Cat_EmployeeGroupMultiEntity, Cat_EmployeeGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_EmployeeGroup_Multi);
        }

        public JsonResult GetMultiTypeOfReplace(string text)
        {
            return GetDataForControl<Cat_TypeOfReplaceMultiModel, Cat_TypeOfReplaceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TypeOfReplace_Multi);
        }

        public JsonResult GetMultiSubject(string text)
        {
            return GetDataForControl<CatSubjectMultiModel, Cat_SubjectMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Subject_Multi);
        }

        public JsonResult GetMultiLockObject(string text)
        {
            return GetDataForControl<CatLockObjectMultiModel, Cat_LockObjectMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LockObject_Multi);

            //return GetDataForControl<HreAppendixContractTypeMultiModel, hre Cat_ContrExportAllCatShiftlListactTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_AppendixContractType_multi);
        }

        #endregion

        #region Cat_ReasonDeny
        public ActionResult GetReasonDenyList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ResonDeny);
        }
        public ActionResult ExportReasonDenySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ResonDenyByIds);
        }
        public ActionResult ExportAllReasonDenylList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ResonDeny);
        }
        public JsonResult GetMultiReasonDeny(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ReasonDeny_Multi);
        }
        #endregion

        #region Cat_LevelGeneral

        /// <summary>
        /// [Quoc.Do] - Lấy danh sách dữ liệu bảng Trình Độ theo type (Cat_NameEntity)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLevelGeneralList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }

        [HttpPost]
        public ActionResult GetLockObjectGeneralList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LockObject);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Trình Độ theo type (Cat_NameEntity) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllLevelGeneralList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn của Trình Độ theo type (Cat_NameEntity) theo điều kiện tìm kiếm
        public ActionResult ExportLevelGeneralSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }

        #endregion

        #region Cat_BlackListReson
        public ActionResult GetBlackListResonList([DataSourceRequest] DataSourceRequest request, Cat_BlackListResonSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_BlackListResonSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllBlackListResonList([DataSourceRequest] DataSourceRequest request, Cat_BlackListResonSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_BlackListResonSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        public ActionResult ExportBlackListResonSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }

        public ActionResult GetMultiResonBlackList(string text)
        {
            return GetDataForControl<CatNameEntityModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_BlackListReason_Multi);
        }
        #endregion
        #region Med_AnnualHealthTime
        public ActionResult GetAnnualHealthTimeList([DataSourceRequest] DataSourceRequest request, Med_AnnualHealthTimeSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Med_AnnualHealthTimeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ReasonChangeSalary);
        }
        public ActionResult ExportMed_AnnualHealthTimeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }

        public JsonResult GetMultiAnnualHealthTime(string text)
        {
            return GetDataForControl<Med_AnnualHealthTimeMultiModel, Med_AnnualHealthTimeMultiEntity>(text, ConstantSql.hrm_med_sp_get_AnnualHealthTime_multi);
        }
        #endregion
        #region Cat_ReasonChangeSalary
        public ActionResult GetReasonChangeSalaryList([DataSourceRequest] DataSourceRequest request, Cat_ReasonChangeSalarySearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_ReasonChangeSalarySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ReasonChangeSalary);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllReasonChangeSalaryList([DataSourceRequest] DataSourceRequest request, Cat_ReasonChangeSalarySearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_ReasonChangeSalarySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ReasonChangeSalary);
        }
        public ActionResult ExportReasonChangeSalarySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }


        #endregion

        #region Cat_TimeEvalutionData
        public ActionResult GetTimeEvalutionDataList([DataSourceRequest]DataSourceRequest request, Cat_TimeEvalutionDataSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_TimeEvalutionDataSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        public ActionResult ExportAllTimeEvalutionDataList([DataSourceRequest]DataSourceRequest request, Cat_TimeEvalutionDataSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_TimeEvalutionDataSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        public ActionResult ExportTimeEvalutionDataSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }
        public ActionResult GetMultiTimeEvalutionData(string text)
        {
            return GetDataForControl<CatNameEntityModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TimeEvalutionData_Multi);
        }
        #endregion

        #region Cat_AreaPostJob - Vùng đăng tuyển

        public ActionResult GetAreaPostJobList([DataSourceRequest] DataSourceRequest request, Cat_AreaPostJobSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_AreaPostJobSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        // [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllAreaPostJobList([DataSourceRequest] DataSourceRequest request, Cat_AreaPostJobSearchModel model)
        {
            return ExportAllAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_AreaPostJobSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        public ActionResult ExportAreaPostJobListSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }

        public ActionResult GetMultiAreaPostJob(string text)
        {
            return GetDataForControl<CatNameEntityModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_AreaPostJob_Multi);
        }

        #endregion

        #region Cat_TypeOfStop
        public ActionResult GetTypeOfStopList([DataSourceRequest]DataSourceRequest request, Cat_TypeOfStopSearchModel model)
        {
            return GetListDataAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_TypeOfStopSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        public ActionResult ExportAllTypeOfStopList([DataSourceRequest]DataSourceRequest request, Cat_TypeOfStopSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_TypeOfStopSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        public ActionResult ExportTypeOfStopSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }
        public ActionResult GetMultiTypeOfStop(string text)
        {
            return GetDataForControl<CatNameEntityModel, Cat_NameEntityEntity>(text, ConstantSql.hrm_cat_sp_get_TypeOfStop_Multi);
        }

        // Task  0049458 - Hard Code với mã là TerminateContract
        public JsonResult GetMultiTypeOfStopResign(string text)
        {
            string status = string.Empty;
            ActionService ActionService = new ActionService(UserLogin);
            var objs = new List<object>();
            objs.Add(text);
            objs.Add("E_TYPEOFSTOP");
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = ActionService.GetData<Cat_NameEntityEntity>(objs, ConstantSql.hrm_cat_sp_get_LevelGeneral, ref status).Where(s => s.Code == "TerminateContract").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_CutOffDurationType
        [HttpPost]
        public ActionResult GetCutOffDurationTypeList([DataSourceRequest] DataSourceRequest request, Cat_CutOffDurationTypeSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_CutOffDurationTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CutOffDurationType);
        }
        #endregion

        #region Cat_ConditionalColor
        [HttpPost]
        public ActionResult GetCatConditionalColor([DataSourceRequest] DataSourceRequest request, CatConditionalColorSearchModel model)
        {
            return GetListDataAndReturn<Cat_ConditionalColorModel, Cat_ConditionalColorEntity, CatConditionalColorSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_conditioncolor);
        }

        public ActionResult ExportCatConditionalColorSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ConditionalColorEntity, Cat_ConditionalColorModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_conditioncolorByIds);
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllCatConditionalColor([DataSourceRequest] DataSourceRequest request, CatConditionalColorSearchModel model)
        {
            return ExportAllAndReturn<Cat_ConditionalColorEntity, Cat_ConditionalColorModel, CatConditionalColorSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_conditioncolor);
        }
        #endregion

        #region Cat_Pivot
        public ActionResult GetMultiPivot(string text)
        {
            return GetDataForControl<Cat_PivotMultiModel, Cat_PivotMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Pivot_Multi);
        }
        #endregion

        #region Cat_PITFormula
        public JsonResult GetMultiPITFormula(string text)
        {
            return GetDataForControl<Cat_PITFormulaMultiEntity, Cat_PITFormulaMultiEntity>(text, ConstantSql.hrm_cat_sp_get_PITFormula_Multi);
        }
        public ActionResult GetPITFormulaList([DataSourceRequest] DataSourceRequest request, Cat_PITFormulaSearchModel model)
        {
            return GetListDataAndReturn<Cat_PITFormulaModel, Cat_PITFormulaEntity, Cat_PITFormulaSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PITFormula);
        }
        public ActionResult ExportPITFormulaSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PITFormulaEntity, Cat_PITFormulaModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PITFormulaByIds);
        }
        public ActionResult ExportAllPITFormulaList([DataSourceRequest] DataSourceRequest request, Cat_PITFormulaSearchModel model)
        {
            return ExportAllAndReturn<Cat_PITFormulaEntity, Cat_PITFormulaModel, Cat_PITFormulaSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PITFormula);
        }
        #endregion
        #region Cat_PITConfig
        public ActionResult GetPITConfigByPITFormularID([DataSourceRequest] DataSourceRequest request, Guid? PITFormularID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            string _PITFormularID = string.Empty;
            if (PITFormularID != Guid.Empty)
            {
                _PITFormularID = Common.DotNetToOracle(PITFormularID.ToString());
            }
            objs.Add(_PITFormularID);
            var result = baseService.GetData<Cat_PITConfigEntity>(objs, ConstantSql.hrm_cat_sp_get_PITConfigByPITFormularID, UserLogin, ref status);
            if (result != null)
            {
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }
        #endregion

        #region Cat_ImportAtt
        public ActionResult GetCatImportAtt([DataSourceRequest] DataSourceRequest request, Cat_ImportAttSearchModel model)
        {
            return GetListDataAndReturn<Cat_ImportAttModel, Cat_ImportAttEntity, Cat_ImportAttSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ImportAtt);
        }
        #endregion

        #region Cat_ImportAttDetail
        public ActionResult GetImportAttDetailByImportAttID([DataSourceRequest] DataSourceRequest request, Guid? ImportAttID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            string _ImportAttID = string.Empty;
            if (ImportAttID != Guid.Empty)
            {
                _ImportAttID = Common.DotNetToOracle(ImportAttID.ToString());
            }
            objs.Add(_ImportAttID);
            var result = baseService.GetData<Cat_ImportAttDetailEntity>(objs, ConstantSql.hrm_cat_sp_get_ImportAttDetailByImportAttid, UserLogin, ref status);
            if (result != null)
            {
                return Json(result.ToDataSourceResult(request));
            }

            return null;
        }
        public ActionResult GetShowWorkHourByShift(Guid? ShiftID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            //var objShift = new List<object>();
            //objShift.Add(ShiftID);
            var result = baseService.GetData<Cat_ShiftEntity>(ShiftID, ConstantSql.hrm_cat_sp_get_ShiftById, UserLogin, ref status);
            if (result != null)
            {
                var _WorkHours = result[0].WorkHours;
                return Json(_WorkHours);
            }
            return null;
        }

        #endregion
        #region Cat_Import
        [HttpPost]
        public ActionResult GetCatImport([DataSourceRequest] DataSourceRequest request, Cat_ImportSearchModel model)
        {
            return GetListDataAndReturn<CatImportModel, CatImportModel, Cat_ImportSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Import);
        }

        public JsonResult GetMultiImport(string text)
        {
            string keyword = string.Empty;
            string obj = string.Empty;
            var arrText = text.Split('|');
            keyword = arrText[0];
            obj = arrText[1];

            if (arrText.Count() == 2 && obj != string.Empty)
            {
                if (keyword == null || keyword == " ")
                    keyword = string.Empty;
                var service = new BaseService();
                string status = string.Empty;
                var listEntity = service.GetData<Cat_ImportMultiEntity>(keyword, ConstantSql.hrm_cat_sp_get_Import_multi, UserLogin, ref status);
                if (listEntity != null)
                {
                    listEntity = listEntity.Where(s => s.ObjectName == obj).ToList();
                    List<CatImportMultiModel> listModel = listEntity.Translate<CatImportMultiModel>();
                    return Json(listModel, JsonRequestBehavior.AllowGet);
                }
                return Json(status, JsonRequestBehavior.AllowGet);
            }

            return GetDataForControl<CatImportMultiModel, Cat_ImportMultiEntity>(keyword, ConstantSql.hrm_cat_sp_get_Import_multi);
        }

        /// <summary>Lấy danh sách đối tượng (trong db) [TRACKING_ITEM.xml]</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult GetMultiObjectName(string text)
        {
            List<CatSysTablesMultiModel> listModel = new List<CatSysTablesMultiModel>();
            listModel = GetObjectNames(false);
            if (!string.IsNullOrWhiteSpace(text))
            {
                listModel = listModel.Where(d => d.Name.GetString().ToLower().Contains(text.ToLower().Trim())).ToList();
            }
            return Json(listModel.OrderBy(d => d.Name).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Lấy danh sách tất cả đối tượng (trong db) [TRACKING_ITEM.xml]</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult GetMultiObjectNameAll(string text)
        {
            List<CatSysTablesMultiModel> listModel = new List<CatSysTablesMultiModel>();
            listModel = GetObjectNames(true);
            if (!string.IsNullOrWhiteSpace(text))
            {
                listModel = listModel.Where(d => d.Name.GetString().ToLower().Contains(text.ToLower().Trim())).ToList();
            }
            return Json(listModel.OrderBy(d => d.Name).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Lấy danh sách tất cả đối tượng (trong db) [TRACKING_ITEM.xml]</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult GetMultiObjectNameForCategory(string text)
        {
            List<CatSysTablesMultiModel> listModel = new List<CatSysTablesMultiModel>();
            listModel = GetObjectNames(true);
            listModel = listModel.Where(m => m.Name.Contains("Cat_")).ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                listModel = listModel.Where(d => d.Name.GetString().ToLower().Contains(text.ToLower().Trim())).ToList();
            }
            return Json(listModel.OrderBy(d => d.Name).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Lấy danh Object Name trong file TRACKING_ITEM.xml</summary>
        /// <param name="isGetAllObjectName">true : lấy tất cả đối tượng (bao gồm những đối tượng không log ghi log) </param>
        /// <returns></returns>
        private List<CatSysTablesMultiModel> GetObjectNames(bool isGetAllObjectName = false)
        {
            /*
            * ◆ Goal(Lấy danh sách Object Name trong file TRACKING_ITEM.xml)
            * ◆ Steps :
            *     ● Step1  :  lấy tất cả dữ liệu trong TRACKING_ITEM.xml
            *     ● Step2  :  Hoặc lấy những dữ liệu trong TRACKING_ITEM.xml ( đối với thuộc tính ghi log Create,Update,Delete là true)
            */
            //[Tung.Ly 20160225]
            var itemTracking = new ItemTrackingManager();
            itemTracking.SettingPath = Common.GetPath(Constant.Settings);
            var listItemTracking = itemTracking.GetSettings();
            List<CatSysTablesMultiModel> listModel = new List<CatSysTablesMultiModel>();
            if (listItemTracking != null && listItemTracking.Count() > 0)
            {
                if (isGetAllObjectName)
                {
                    listModel = listItemTracking.Select(d => new CatSysTablesMultiModel { Name = d.Name }).ToList();
                }
                else
                {
                    //các thao tác thực hiện khi ghi log (create,update,delete)
                    listModel = listItemTracking.Where(d => d.Create || d.Update || d.Delete)
                        .Select(d => new CatSysTablesMultiModel { Name = d.Name }).ToList();
                }
            }
            return listModel;
        }

        public JsonResult GetMultiFieldNameByTableName(string text)
        {
            var listObj = new List<object> { text, 1, 500 };
            return GetData<CatSysTablesMultiModel, Cat_SysTablesMultiEntity>(listObj, ConstantSql.hrm_cat_sp_get_FieldNameByTableName);
        }

        public JsonResult GetMultiObjectNameByObjName(string text)
        {
            if (text == string.Empty)
            {
                text = null;
            }

            var listObj = new List<object> { text, 1, 1000 };
            return GetData<CatSysTablesMultiModel, Cat_SysTablesMultiEntity>(listObj, ConstantSql.hrm_cat_sp_get_tablesByTableName);
        }

        /// <summary>
        /// Dữ liệu sai dùng để export ra lại excel.
        /// </summary>
        private static Dictionary<Guid, ProgressEventArgs> ListComputePercent { get; set; }

        public ActionResult ImportValidate(CatImportModel Model)
        {
            if (ListComputePercent != null && ListComputePercent.ContainsKey(Model.UserID))
            {
                ListComputePercent.Remove(Model.UserID);
            }

            #region Validate
            string message = string.Empty;
            var checkValidate = HRM.Business.Main.Domain.ValidatorService.OnValidateData<CatImportModel>(LanguageCode, Model, "Cat_Import_List", ref message);
            if (!checkValidate)
            {
                var ls = new object[] { "error", message };
                return Json(ls, JsonRequestBehavior.AllowGet);
            }
            #endregion

            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportResultInterviewValidate(CatImportModel Model)
        {
            //if (ListComputePercent != null && ListComputePercent.ContainsKey(Model.UserID))
            //{
            //    ListComputePercent.Remove(Model.UserID);
            //}

            //#region Validate
            string message = string.Empty;
            //var checkValidate = HRM.Business.Main.Domain.ValidatorService.OnValidateData<CatImportModel>(Model, "Cat_Import_List", ref message);
            //if (!checkValidate)
            //{
            //    var ls = new object[] { "error", message };
            //    return Json(ls);
            //}
            //#endregion

            return Json(message);
        }

        public ContentResult ProgessChanged([DataSourceRequest] DataSourceRequest request, CatImportModel model)
        {
            if (ListComputePercent != null && ListComputePercent.ContainsKey(model.UserID))
            {
                var arg = ListComputePercent[model.UserID];
                model.Percent = arg.Percent.ToString();
                model.ProcessName = arg.Name;
                model.ProcessNameView = arg.Value;
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue - 1;
            var result = new ContentResult();
            result.Content = serializer.Serialize(model);
            result.ContentType = "text/json";
            return result;
        }

        [HttpPost]
        public ContentResult Import([DataSourceRequest] DataSourceRequest request, CatImportModel model)
        {
            var _fileName = Common.GetPath(Common.TemplateURL) + model.TemplateFile;
            _fileName = _fileName.Replace("/", "\\");

            ImportService importService = new ImportService
            {
                FileName = _fileName,
                UserID = model.UserID,
                ImportTemplateID = model.ID,
                DateTimeFormat = model.FormatDate,
                ImportMode = model.ProcessDuplicateData == HRM.Infrastructure.Utilities.EnumDropDown.DuplicateData.E_INSERT.ToString() ?
                ImportDataMode.Insert : model.ProcessDuplicateData == HRM.Infrastructure.Utilities.EnumDropDown.DuplicateData.E_UPDATE.ToString() ?
                ImportDataMode.Update : ImportDataMode.Skip
            };

            importService.ProgressChanged += importService_ProgressChanged;
            DataTable dataTableInvalid = new DataTable("InvalidData");
            DataTable dataTable = new DataTable("ImportData");
            DataTable dataTableError = new DataTable("ErrorData");

            string[] lstFieldInvalid = new string[]
        {
                "DataField",
                "InvalidValue",
                "ExcelField",
                "ExcelValue",
                "ValueType",
                "Desciption"
        };

            try
            {
                importService.Import();

                dataTable = importService.GetImportObject().Translate(importService.ListValueField.ToArray());
                var invalidData = importService.GetInvalidObject();
                dataTableInvalid = invalidData.Translate(lstFieldInvalid);
                var lstValueField = importService.ListValueField;
                lstValueField.Add(Constant.ErrorDes);
                var lstErrorData = invalidData.Select(d => d.ImportData).Distinct().ToList();
                dataTableError = lstErrorData.Translate(lstValueField.ToArray());
                if (model.UserID == Guid.Empty)
                {
                    model.Description = "Người dùng ảo";
                }

                if (model.IsImportAndSave)
                {
                    model.Description = importService.Save(UserLogin);
                    //if (importService.Save(UserLogin))
                    //{
                    //    model.Description = NotificationType.Success.ToString();
                    //}
                    //else
                    //{
                    //    model.Description = NotificationType.Error.ToString();
                    //}
                }
            }
            catch (Exception ex)
            {
                model.Description = ex.Message;
            }

            model.ListImportData = dataTable.ConfigTable().ToDataSourceResult(request);
            model.ListInvalidData = dataTableInvalid.ConfigTable().ToDataSourceResult(request);
            model.ListErrorData = dataTableError.ConfigTable().ToDataSourceResult(request);
            model.UrlInvalidFileName = ExportInvalidData(model.UserID, model.ID, importService);

            model.ListValueField = importService.ListValueField;
            model.ListDisplayField = lstFieldInvalid.ToList();

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue - 1;
            var result = new ContentResult();
            result.Content = serializer.Serialize(model);
            result.ContentType = "text/json";
            return result;
        }

        public ActionResult SaveImport([DataSourceRequest] DataSourceRequest request, CatImportModel model)
        {
            try
            {
                if (model.UserID != Guid.Empty)
                {
                    var _fileName = Common.GetPath(Common.TemplateURL) + model.TemplateFile;
                    _fileName = _fileName.Replace("/", "\\");
                    ImportService importService = new ImportService
                    {
                        UserID = model.UserID,
                        FileName = _fileName,
                        ImportTemplateID = model.ID,
                        DateTimeFormat = model.FormatDate,
                        ImportMode = model.ProcessDuplicateData == HRM.Infrastructure.Utilities.EnumDropDown.DuplicateData.E_INSERT.ToString() ?
                            ImportDataMode.Insert : model.ProcessDuplicateData == HRM.Infrastructure.Utilities.EnumDropDown.DuplicateData.E_UPDATE.ToString() ?
                            ImportDataMode.Update : ImportDataMode.Skip
                    };

                    importService.ProgressChanged += importService_ProgressChanged;
                    model.Description = importService.Save(UserLogin);

                    #region Sau khi luu du lieu import => xoa file import
                    try
                    {
                        if (model.Description == NotificationType.Success.ToString())
                        {
                            if (System.IO.File.Exists(_fileName))
                            {
                                System.IO.File.Delete(_fileName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    #endregion

                    return Json(model.Description);
                }
                else
                {
                    model.Description = "Người dùng ảo";
                    return Json(NotificationType.Error.ToString());
                }
            }
            catch (Exception ex)
            {
                return Json(NotificationType.Error + "," + ex.GetExceptionMessage());
            }
        }

        public ContentResult ImportNotConfig([DataSourceRequest] DataSourceRequest request, CatImportModel model)
        {
            var _fileName = Common.GetPath(Common.TemplateURL) + model.TemplateFile;
            _fileName = _fileName.Replace("/", "\\");
            ImportService importService = new ImportService
            {
                FileName = _fileName,
                UserID = Guid.Parse(UserID),
                ImportTemplateID = null,
                DateTimeFormat = model.FormatDate,
                ImportMode = model.ProcessDuplicateData == HRM.Infrastructure.Utilities.EnumDropDown.DuplicateData.E_INSERT.ToString() ?
                ImportDataMode.Insert : model.ProcessDuplicateData == HRM.Infrastructure.Utilities.EnumDropDown.DuplicateData.E_UPDATE.ToString() ?
                ImportDataMode.Update : ImportDataMode.Skip
            };
            importService.ProgressChanged += importService_ProgressChanged;
            DataTable dataTableInvalid = new DataTable("InvalidData");
            DataTable dataTable = new DataTable("ImportData");

            string[] lstFieldInvalid = new string[]
        {
                "DataField",
                "InvalidValue",
                "ExcelField",
                "ExcelValue",
                "ValueType",
                "Desciption"
        };

            //try
            //{
            string message = importService.GetMappingImportData(model.ObjectName, _fileName);
            if (!string.IsNullOrEmpty(message))
            {
                model.Description = message;
                var serializerTemp = new JavaScriptSerializer();
                serializerTemp.MaxJsonLength = Int32.MaxValue - 1;
                var resultTemp = new ContentResult();
                resultTemp.Content = serializerTemp.Serialize(model);
                resultTemp.ContentType = "text/json";
                return resultTemp;
            }
            importService.Import();

            dataTable = importService.GetImportObject().Translate(importService.ListValueField.ToArray());
            dataTableInvalid = importService.GetInvalidObject().Translate(lstFieldInvalid);
            if (model.UserID == Guid.Empty)
            {
                model.Description = "Người dùng ảo";
            }

            if (model.IsImportAndSave)
            {
                model.Description = importService.Save(UserLogin);
                //if (importService.Save(UserLogin))
                //{
                //    model.Description = NotificationType.Success.ToString();
                //}
                //else
                //{
                //    model.Description = NotificationType.Error.ToString();
                //}
            }
            //}
            //catch (Exception ex)
            //{
            //    model.Description = ex.Message;
            //}
            model.ListImportData = dataTable.ConfigTable().ToDataSourceResult(request);
            model.ListInvalidData = dataTableInvalid.ConfigTable().ToDataSourceResult(request);
            model.UrlInvalidFileName = ExportInvalidData(model.UserID, model.ID, importService);

            model.ListValueField = importService.ListValueField;
            model.ListDisplayField = lstFieldInvalid.ToList();

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue - 1;
            var result = new ContentResult();
            result.Content = serializer.Serialize(model);
            result.ContentType = "text/json";
            return result;
        }
        public string ExportInvalidData(Guid userID, Guid templateID, ImportService importService)
        {
            var result = string.Empty;

            try
            {
                var listInvalidData = importService.GetInvalidObject().Select(d => d.ImportData).Distinct().ToList();

                if (userID != Guid.Empty && listInvalidData != null && listInvalidData.Count > 0)
                {
                    result = importService.Export(templateID, listInvalidData, Common.GetPath(Common.DownloadURL));
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }


        void importService_ProgressChanged(ProgressEventArgs e)
        {
            if (e != null && !string.IsNullOrWhiteSpace(e.Value))
            {
                if (ListComputePercent == null)
                {
                    ListComputePercent = new Dictionary<Guid, ProgressEventArgs>();
                }

                if (ListComputePercent.ContainsKey(e.ID))
                {
                    ListComputePercent[e.ID] = e;
                }
                else
                {
                    ListComputePercent.Add(e.ID, e);
                }
            }
        }

        [HttpPost]
        public ActionResult ConvertFromPivot([DataSourceRequest] DataSourceRequest request, Cat_PivotModel model)
        {
            var _fileName = Common.GetPath(Common.TemplateURL) + model.PivotFileName;
            _fileName = _fileName.Replace("/", "\\");
            string[] outputFiles = null;

            UnpivotService service = new UnpivotService
            {
                UserID = model.UserID,
                PivotTemplateID = model.ID,
                FileName = _fileName,
            };

            try
            {
                outputFiles = service.Unpivot();
            }
            catch (Exception ex)
            {
                model.Description = ex.GetExceptionMessage();
                return Json(model.Description, JsonRequestBehavior.AllowGet);
            }

            if (outputFiles != null && outputFiles.Count() > 0 && !string.IsNullOrEmpty(outputFiles[0]))
            {
                outputFiles[0] = NotificationType.Success.ToString() + "," + outputFiles[0];
                return Json(outputFiles[0], JsonRequestBehavior.AllowGet);
            }

            return Json("-1", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_WorkPlace
        public JsonResult GetDataWorkPlace(Guid? ID)
        {
            if (ID != null && ID != Guid.Empty)
            {
                var Services = new ActionService(UserLogin);
                string status = string.Empty;
                var workPlaceEntity = Services.GetByIdUseStore<Cat_WorkPlaceEntity>(ID.Value, ConstantSql.hrm_cat_sp_get_WorkPlaceById, ref status);
                return Json(workPlaceEntity, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }
        public ActionResult GetWorkPlaceList([DataSourceRequest] DataSourceRequest request, CatWorkPlaceSearchModel model)
        {
            return GetListDataAndReturn<CatWorkPlaceModel, Cat_WorkPlaceEntity, CatWorkPlaceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_WorkPlace);
        }

        public JsonResult GetWorkPlaceOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_WorkPlaceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_WorkPlace_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<CatWorkPlaceMultihModel> listModel = listEntity.Translate<CatWorkPlaceMultihModel>();
                listModel = listModel.OrderBy(s => s.WorkPlaceName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiWorkPlace(string text)
        {
            return GetDataForControl<CatWorkPlaceMultihModel, Cat_WorkPlaceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_WorkPlace_Multi);
        }

        public JsonResult GetMultiLocation(string text)
        {
            return GetDataForControl<Cat_LocationMultiModel, Cat_LocationMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Location_Multi);
        }

        public JsonResult GetMultiNameEntity(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_NameEntity_Multi);
        }
        public JsonResult GetAttRosterGroupV2(string text)
        {
            string status = string.Empty;
            Sys_AttOvertimePermitConfigServices sysServices = new Sys_AttOvertimePermitConfigServices();
            string listcode = sysServices.GetConfigValue<string>(AppConfig.HRM_ATT_CONFIG_NAME_ROSTERGROUP);
            if (string.IsNullOrEmpty(listcode))
            {
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            var array = listcode.Split(',').ToList();
            if (text == null || text == " ")
                text = string.Empty;

            if (!Common.CheckListNullOrEmty(array))
            {
                var listmodel = new List<Att_RosterGroupV2Model>();
                foreach (var item in array)
                {
                    if (text != string.Empty)
                    {
                        var _ITEM = item.ToUpper();
                        if (_ITEM.IndexOf(text.ToUpper()) != -1)
                        {
                            Att_RosterGroupV2Model _NAME = new Att_RosterGroupV2Model();
                            _NAME.RosterGroupName = item;
                            listmodel.Add(_NAME);
                        }
                    }
                    else
                    {
                        Att_RosterGroupV2Model _NAME = new Att_RosterGroupV2Model();
                        _NAME.RosterGroupName = item;
                        listmodel.Add(_NAME);
                    }
                }
                return new JsonResult() { Data = listmodel, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }


        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_WorkPlace(Cat_WorkPlace) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllWorkPlaceList([DataSourceRequest] DataSourceRequest request, CatWorkPlaceSearchModel model)
        {
            return ExportAllAndReturn<Cat_WorkPlaceEntity, CatWorkPlaceModel, CatWorkPlaceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_WorkPlace);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_WorkPlace (Cat_WorkPlace) theo điều kiện tìm kiếm
        public ActionResult ExportWorkPlaceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_WorkPlaceEntity, CatWorkPlaceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        #endregion

        #region Cat_HDTJob

        [HttpPost]
        public ActionResult ApprovedHDTJobType(string selectedIds)
        {
            var service = new Cat_HDTJobTypeServices();
            var message = service.ActionApproved(selectedIds);
            return Json(message);
        }

        [HttpPost]
        public ActionResult GetHDTJobList([DataSourceRequest] DataSourceRequest request, Cat_HDTJobTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_HDTJobTypeModel, Cat_HDTJobTypeEntity, Cat_HDTJobTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_HDTJobType);
        }

        [HttpPost]
        public ActionResult ApprovedHDTJobAll([DataSourceRequest] DataSourceRequest request, Cat_HDTJobTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_HDTJobTypeModel, Cat_HDTJobTypeEntity, Cat_HDTJobTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_set_ApprovedAllHDTJobType);
        }

        public JsonResult GetMultiHDTJob(string text)
        {
            return GetDataForControl<Cat_HDTJobTypeMultihModel, Cat_HDTJobTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_HDTJobType_Multi);
        }
        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_HDTJob(Cat_HDTJob) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllHDTJobTypeList([DataSourceRequest] DataSourceRequest request, Cat_HDTJobTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_HDTJobTypeEntity, Cat_HDTJobTypeModel, Cat_HDTJobTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_HDTJobType);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_HDTJob (Cat_HDTJob) theo điều kiện tìm kiếm
        public ActionResult ExportHDTJobTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_HDTJobTypeEntity, Cat_HDTJobTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        #endregion

        #region Cat_Qualification
        public JsonResult GetMultiQualification(string text)
        {
            return GetDataForControl<CatQualificationMultiModel, Cat_QualificationMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Qualification_Multi);
        }

        public JsonResult GetMultiQualificationLevel(string text)
        {
            //return GetDataForControl<CatQualificationLevelMultiModel, Cat_QualificationMultiLevelEntity>(text, ConstantSql.hrm_cat_sp_get_QualificationLevel_Multi);
            string status = string.Empty;
            var services = new ActionService(LanguageCode);
            var obj = new List<object>();
            obj.AddRange(new object[4]);
            obj[0] = text;
            obj[1] = "E_QUALIFICATION_LEVEL";
            obj[2] = 1;
            obj[3] = int.MaxValue - 1;
            var lstNameEntity = baseService.GetData<Cat_NameEntityEntity>(obj, ConstantSql.hrm_cat_sp_get_LevelGeneral, UserLogin, ref status);
            if (lstNameEntity != null)
            {
                lstNameEntity = lstNameEntity.OrderBy(s => s.Order).ToList();
            }
            return Json(lstNameEntity, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region
        //public ActionResult CreateInCellSkillCourseCertificate([Bind(Prefix = "models")] List<Cat_SkillCourseCertificateModel> model)
        //{
        //    var service = new BaseService();
        //    var status = string.Empty;
        //    if (model != null)
        //    {
        //        foreach (var i in model)
        //        {
        //            if (i.ID != Guid.Empty)
        //            {
        //                status = service.Edit<Cat_SkillCourseCertificateEntity>(i.CopyData<Cat_SkillCourseCertificateEntity>());
        //            }
        //            else
        //            {
        //                status = service.Add<Cat_SkillCourseCertificateEntity>(i.CopyData<Cat_SkillCourseCertificateEntity>());
        //            }
        //        }
        //    }
        //    return Json(status);
        //}

        //public ActionResult DeleteInCellSkillCourseCertificate([Bind(Prefix = "models")] List<Cat_SkillCourseCertificateModel> model)
        //{
        //    ActionService service = new ActionService(UserLogin, LanguageCode);
        //    if (model != null)
        //    {
        //        foreach (var i in model)
        //        {
        //            var result = service.DeleteOrRemove<Cat_SkillCourseCertificateEntity, Cat_SkillCourseCertificateModel>(DeleteType.Remove.ToString() + "," + Common.DotNetToOracle(i.ID.ToString()));
        //        }
        //    }

        //    return Json("");
        //}
        #endregion

        public JsonResult GetScreenName(string screenName)
        {
            var service = new Cat_ExportServices();
            string status = string.Empty;
            var result = service.GetData<CatExportModel>(screenName, ConstantSql.hrm_cat_sp_get_Export_multi, UserLogin, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>Lấy danh sách xuất báo cáo (loại trừ những bc không tìm thấy file xuất bc)</summary>
        /// <param name="screenName">Tên màn hình theo dạng controller/action (vd: Hre_Profile/Index)</param>
        /// <returns></returns>
        public JsonResult GetExportByScreenName(string screenName)
        {
            //[Tung.Ly 20170927]: Lấy danh sách xuất báo cáo (loại trừ những bc không tìm thấy file xuất bc để thuận tiện cho người dùng đỡ mất thời gian khi file template không tồn tại)

            /*
            * ◆ Goal(Lấy danh sách xuất báo cáo (loại trừ những bc không tìm thấy file xuất bc))
            * ◆ Steps :
            *     ● Step1  :  xử lý tương tự GetScreenName(string screenName)
            *     ● Step2  :    + Thêm phần xử lý kiểm những file không tồn tại sẽ loại ra khỏi danh sách
            */


            var service = new Cat_ExportServices();
            string status = string.Empty;
            var result = service.GetData<CatExportModel>(screenName, ConstantSql.hrm_cat_sp_get_Export_multi, UserLogin, ref status);

            #region chỉ lấy danh sách templateFile tồn tại (loại bỏ tất cả những catExport không tồn tại templateFile)
            string templatepath = Common.GetPath(Common.TemplateURL);
            var lstCatExport = new List<CatExportModel>();
            foreach (var item in result)
            {
                string fileExt = templatepath + item.TemplateFile;
                bool fileExists = (System.IO.File.Exists(fileExt) ? true : false);
                if (fileExists)
                {
                    lstCatExport.Add(item);
                }
            }
            #endregion

            return Json(lstCatExport, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ChangeApproved(string Ids)
        {
            Pur_MCAMService serv = new Pur_MCAMService();
            var message = serv.ChangeApproved(Ids, UserLogin);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePaidDeposit(string selectedIds)
        {
            Pur_MCAMService serv = new Pur_MCAMService();
            var message = serv.ChangePaidDeposit(selectedIds, UserLogin);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeCancel(string selectedIds)
        {
            Pur_MCAMService serv = new Pur_MCAMService();
            var message = serv.ChangeCancel(selectedIds, UserLogin);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckByCar(string Ids)
        {
            Pur_MCAMService serv = new Pur_MCAMService();
            var message = serv.CheckBuyCar(Ids, UserLogin);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScreenNameWord(string screenName)
        {
            var service = new Cat_ExportServices();
            string status = string.Empty;
            var result = service.GetData<CatExportModel>(screenName, ConstantSql.hrm_cat_sp_get_ExportWord_multi, UserLogin, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScreenNameWordContract(string screenName)
        {
            var service = new Cat_ExportServices();
            string status = string.Empty;
            var result = service.GetScreenNameWordContract(screenName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Thong.Huynh] - 2014/05/28
        /// Lấy dữ liệu load lên lưới bằng cùng điều kiện tìm kiếm
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        #region GetRelativeTypeList
        //public JsonResult GetRelativeTypeList([DataSourceRequest] DataSourceRequest request, CatRelativeTypeSearchModel searchModel)
        //{
        //    var service = new Cat_RelativeTypeServices();
        //    ListQueryModel model = new ListQueryModel
        //    {
        //        PageIndex = request.Page,
        //        Filters = ExtractFilterAttributes(request),
        //        Sorts = ExtractSortAttributes(request),
        //        AdvanceFilters = ExtractAdvanceFilterAttributes(searchModel)
        //    };
        //    var result = service.GetRelativeType(model).ToList().Translate<CatRelativeTypeModel>();
        //    if (searchModel.IsExport)
        //    {
        //        var fullPath = ExportService.Export(result, searchModel.ValueFields.Split(','));
        //        return Json(fullPath);
        //    }
        //    request.Page = 1;
        //    var dataSourceResult = result.ToDataSourceResult(request);
        //    dataSourceResult.Total = result.Count() <= 0 ? 0 : result.FirstOrDefault().TotalRow;
        //    return Json(dataSourceResult, JsonRequestBehavior.AllowGet);
        //} 
        #endregion

        #region Cat_RelativeType
        public ActionResult GetRelativeTypeList([DataSourceRequest] DataSourceRequest request, CatRelativeTypeSearchModel searchModel)
        {
            return GetListDataAndReturn<CatRelativeTypeModel, Cat_RelativeTypeEntity, CatRelativeTypeSearchModel>(request, searchModel, ConstantSql.hrm_cat_sp_get_RelativesType);
        }

        /// [Tho.Bui] - Xuất danh sách dữ liệu cho Cat_RelativeType(Cat_RelativeType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllRelativeTypeList([DataSourceRequest] DataSourceRequest request, CatRelativeTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_RelativeTypeEntity, CatRelativeTypeModel, CatRelativeTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RelativesType);
        }

        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn Cat_RelativeType (Cat_RelativeType) theo điều kiện tìm kiếm
        public ActionResult ExportRelativeTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RelativeTypeEntity, CatRelativeTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryClassByIds);
        }
        #endregion


        #region ExportRelativesTypeSelected old
        //public ActionResult ExportRelativesTypeSelected(string selectedIds, string valueFields)
        //{
        //    var service = new Cat_RelativeTypeServices();
        //    var result = service.GetRelativeTypeByIds(selectedIds).Translate<CatRelativeTypeModel>();
        //    var fullPath = ExportService.Export(result, valueFields.Split(','));
        //    return Json(fullPath);
        //} 
        #endregion
        public ActionResult ExportRelativesTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RelativeTypeEntity, CatRelativeTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RelativesTypeIds);
        }

        /// <summary>
        /// [Thong.Huynh] - 2014/05/28
        /// Lấy dữ liệu load lên lưới bằng cùng điều kiện tìm kiếm
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        #region GetCatShiftList old
        //public JsonResult GetCatShiftList([DataSourceRequest] DataSourceRequest request, CatShiftSearchModel model)
        //{
        //    var service = new Cat_ShiftServices();
        //    ListQueryModel lstModel = new ListQueryModel
        //    {
        //        PageIndex = request.Page,
        //        Filters = ExtractFilterAttributes(request),
        //        Sorts = ExtractSortAttributes(request),
        //        AdvanceFilters = ExtractAdvanceFilterAttributes(model)
        //    };
        //    var result = service.GetCat_Shift(lstModel).ToList().Translate<CatShiftModel>();
        //    return Json(result.ToDataSourceResult(request));
        //} 
        #endregion
        public ActionResult GetCatShiftList([DataSourceRequest] DataSourceRequest request, CatShiftSearchModel searchModel)
        {

            return GetListDataAndReturn<CatShiftModel, Cat_ShiftEntity, CatShiftSearchModel>(request, searchModel, ConstantSql.hrm_cat_sp_get_Shift);
        }

        public ActionResult ExportAllCatShiftlList([DataSourceRequest] DataSourceRequest request, CatShiftSearchModel model)
        {
            //return ExportAllAndReturn<Cat_ShiftEntity, CatShiftModel, CatShiftSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Shift);
            #region [Vinh.Mai - 20171214] Format dữ liệu trước khi xuất
            string status = string.Empty;

            var listModel = GetListData<CatShiftModel, Cat_ShiftEntity, CatShiftSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Shift, ref status);
            Dictionary<string, string> formatFields = new Dictionary<string, string>();
            formatFields.Add(CatShiftModel.FieldNames.InTime, ConstantFormat.HRM_Format_HourMin);
            formatFields.Add(CatShiftModel.FieldNames.TimeCoOut, ConstantFormat.HRM_Format_HourMin);

            status = ExportService.Export(listModel, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','), formatFields);
            return Json(status);
            #endregion
        }

        public ActionResult ExportCatShiftSelected(string selectedIds, string valueFields)
        {
            //return ExportSelectedAndReturn<Cat_ShiftEntity, CatShiftModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ShiftByIds);
            #region [Vinh.Mai - 20171214] Format dữ liệu trước khi xuất excel
            string status = string.Empty;
            var lstQuyery = GetData<CatShiftModel, Cat_ShiftEntity>(selectedIds, ConstantSql.hrm_cat_sp_get_ShiftByIds);
            Dictionary<string, string> formatFields = new Dictionary<string, string>();
            formatFields.Add(CatShiftModel.FieldNames.InTime, ConstantFormat.HRM_Format_HourMin);
            formatFields.Add(CatShiftModel.FieldNames.TimeCoOut, ConstantFormat.HRM_Format_HourMin);

            status = ExportService.Export(lstQuyery, valueFields.Split(','), formatFields);
            return Json(status);
            #endregion
        }

        public ActionResult GetCatShiftListDetail([DataSourceRequest] DataSourceRequest request, Cat_ShiftDetailSearchModel searchModel)
        {

            return GetListDataAndReturn<Cat_ShiftDetailModel, Cat_ShiftDetailEntity, Cat_ShiftDetailSearchModel>(request, searchModel, ConstantSql.hrm_cat_sp_get_ShiftDetail);
        }

        public ActionResult ExportAllCatShiftDetailList([DataSourceRequest] DataSourceRequest request, Cat_ShiftDetailSearchModel model)
        {
            return ExportAllAndReturn<Cat_ShiftDetailEntity, Cat_ShiftDetailModel, Cat_ShiftDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ShiftDetail);
        }

        public ActionResult ExportCatShiftDetailSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ShiftDetailEntity, Cat_ShiftDetailModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ShiftDetailByIds);
        }

        #region Cat_Religion
        /// <summary>
        /// [Hieu.Van] - 2014/05/27
        /// Lấy dữ liệu load lên lưới bằng cùng điều kiện tìm kiếm
        /// </summary>
        /// <param name="request"></param>
        /// <param name="otModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetReligionList([DataSourceRequest] DataSourceRequest request, CatReligionSearchModel model)
        {
            return GetListDataAndReturn<CatReligionModel, Cat_ReligionEntity, CatReligionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Religion);
        }

        public ActionResult ExportCatReligionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ReligionEntity, CatReligionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ReligionByIds);
        }

        public ActionResult ExportAllReligionList([DataSourceRequest] DataSourceRequest request, CatReligionSearchModel model)
        {
            return ExportAllAndReturn<Cat_ReligionEntity, CatReligionModel, CatReligionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Religion);
        }


        /// <summary>
        /// Lay61 danh sách tôn giáo
        /// </summary>
        /// <returns></returns>
        public JsonResult GetReligion()
        {
            //var result = service.GetCatReligion().ToList().Translate<CatReligionModel>();
            var result = baseService.GetAllUseEntity<Cat_ReligionEntity>(ref _status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMultiReligion(string text)
        {
            return GetDataForControl<Cat_ReligionMultiModel, Cat_ReligionMultiModel>(text, ConstantSql.hrm_cat_sp_get_religion_Multi);
        }
        #endregion

        /// <summary>
        /// [Thong.Huynh] - 2014/05/28
        /// Lấy dữ liệu load lên lưới bằng cùng điều kiện tìm kiếm
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Model"></param>
        /// <returns></returns>


        /// <summary>
        /// [Tam.Le] - 7.8.2014 - Lấy dữ liệu "Cat_TAMScanReasonMiss","Can_MealAllowanceTypeSetting" theo Id cua Cat_TAMScanReasonMiss
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetTAMScanReasonMiss_ById(Guid Id)
        {

            string status = string.Empty;
            var model = new Cat_TAMScanReasonMissModel();
            ActionService service = new ActionService(UserLogin, LanguageCode);
            var entity = service.GetByIdUseStore<Cat_TAMScanReasonMissEntity>(Id, ConstantSql.hrm_cat_sp_get_TAMScanReasonMiss_ById, ref status);
            if (entity != null)
            {
                model = entity.CopyData<Cat_TAMScanReasonMissModel>();
            }
            return Json(model);
            //var result = new List<Cat_TAMScanReasonMissModel>();
            //string status = string.Empty;
            //if (Id > 0)
            //{
            //    var service = new Cat_TAMScanReasonMissServices();
            //    result = service.GetData<Cat_TAMScanReasonMissModel>(Id, ConstantSql.hrm_cat_sp_get_TAMScanReasonMiss_ById, ref status);

            //}
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Chuc.Nguyen] - 2014/05/30
        /// Lấy danh sách loại nhân viên dùng cho các control có datasource trừ grid
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetCostCentre()
        //{
        //    var service = new Cat_CostCentreServices();
        //    var result = service.Get().ToList().Translate<CatCostCentreModel>();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetShift()
        {
            var service = new Cat_ShiftServices();
            //var data = service.GetCatShift();
            //var data = baseService.GetAllUseEntity<Cat_ShiftEntity>(ref _status);
            var data = baseService.GetDataNotParam<Cat_ShiftMultiEntity>(ConstantSql.hrm_cat_sp_get_Shift_multi, UserLogin, ref _status);
            var result = data.Select(item => new Cat_ShiftMultiEntity()
            {
                ID = item.ID,
                ShiftName = item.ShiftName,
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [Hieu.Van] - 2014/05/27
        /// Lấy dữ liệu load lên lưới bằng cùng điều kiện tìm kiếm
        /// </summary>
        /// <param name="request"></param>
        /// <param name="otModel"></param>
        /// <returns></returns>
        #region GetRegionList
        //[HttpPost]
        //public ActionResult GetRegionList([DataSourceRequest] DataSourceRequest request, CatRegionSearchModel _model)
        //{
        //    var service = new Cat_RegionServices();

        //    ListQueryModel lstModel = new ListQueryModel
        //    {
        //        PageIndex = request.Page,
        //        Filters = ExtractFilterAttributes(request),
        //        Sorts = ExtractSortAttributes(request),
        //        AdvanceFilters = ExtractAdvanceFilterAttributes(_model)
        //    };
        //    var result = service.GetCatRegion(_model.RegionName, _model.Code).ToList().Translate<CatRegionModel>();

        //    return Json(result.ToDataSourceResult(request));
        //} 
        #endregion

        [HttpPost]
        public ActionResult GetRegionList([DataSourceRequest] DataSourceRequest request, CatRegionSearchModel model)
        {
            return GetListDataAndReturn<CatRegionModel, Cat_RegionEntity, CatRegionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Region);
        }

        public ActionResult ExportCatRegionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RegionEntity, CatRegionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RegionByIds);
        }

        public ActionResult ExportAllRegionList([DataSourceRequest] DataSourceRequest request, CatRegionSearchModel model)
        {
            return ExportAllAndReturn<Cat_RegionEntity, CatRegionModel, CatRegionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Region);
        }

        /// <summary>
        /// [Chuc.Nguyen] - 2014/05/27
        /// Lấy dữ liệu load lên lưới bằng cùng điều kiện tìm kiếm cho catbank
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AGetBankList([DataSourceRequest] DataSourceRequest request, CatBankSearchModel model)
        {

            return null;
        }

        [HttpPost]
        public ActionResult GetImportItemByImportIDList([DataSourceRequest] DataSourceRequest request, Guid? ImportID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(ImportID);
            var result = baseService.GetData<Cat_ImportItemEntity>(objs, ConstantSql.hrm_cat_sp_get_ImportItemByImportID, UserLogin, ref status);
            if (result != null)
            {
                return Json(result.ToDataSourceResult(request));
            }

            return null;
        }

        [HttpPost]
        public ActionResult GetSyncItemBySyncIDList([DataSourceRequest] DataSourceRequest request, Guid? SyncID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(SyncID);
            var result = baseService.GetData<Cat_SyncItemEntity>(objs, ConstantSql.hrm_cat_sp_get_SyncItemBySyncID, UserLogin, ref status);
            if (result != null)
            {
                return Json(result.ToDataSourceResult(request));
            }

            return null;
        }

        public JsonResult GetMultiTamScanReasonMiss(string text)
        {
            return GetDataForControl<Cat_TAMScanReasonMissMuitlModel, Cat_TAMScanReasonMissMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TamScanReasonMiss_multi);
        }

        #region GetDataForCostCentre
        [HttpPost]
        public JsonResult GetDataForCostCentre(string _enum, string OrgID, string PosID, string JobID, string OrgWpID)
        {
            string status = string.Empty;
            if (string.IsNullOrEmpty(_enum))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var sysServices = new Sys_AttOvertimePermitConfigServices();
            var Liststr = _enum.Split(',').ToList();
            string _enum1 = sysServices.GetConfigValue<string>(AppConfig.HRM_HRE_CONFIG_ISCOSTCENTREALLOW);
            if (!string.IsNullOrEmpty(_enum1) && Liststr.Contains(_enum1))
            {
                Cat_CostCentreEntity _entity = new Cat_CostCentreEntity();
                Cat_CostCentreServices _ser = new Cat_CostCentreServices();
                if (_enum1 == EnumDropDown.CostCenTreAllow.E_ORGSTRUCTURE.ToString())
                {
                    if (!string.IsNullOrEmpty(OrgID))
                    {
                        _entity = _ser.GetDataFromOrg(Guid.Parse(OrgID));
                    }
                }
                else
                {
                    if (_enum1 == EnumDropDown.CostCenTreAllow.E_POSTION.ToString())
                    {
                        if (!string.IsNullOrEmpty(PosID))
                        {
                            _entity = _ser.GetDataFromPosition(Guid.Parse(PosID));
                        }
                    }
                    else
                    {
                        if (_enum1 == EnumDropDown.CostCenTreAllow.E_JOBTITTLE.ToString())
                        {
                            if (!string.IsNullOrEmpty(JobID))
                            {
                                _entity = _ser.GetDataFromJobTittle(Guid.Parse(JobID));
                            }
                        }
                        else
                        {
                            if (_enum1 == EnumDropDown.CostCenTreAllow.E_ORGSTRUCTUREANDWORKPLACE.ToString())
                            {
                                if (!string.IsNullOrEmpty(OrgWpID) && !string.IsNullOrEmpty(OrgID))
                                {
                                    _entity = _ser.GetDataFromOrgWorkPlace(Guid.Parse(OrgID), Guid.Parse(OrgWpID));
                                }
                            }
                        }
                    }
                }
                if (_entity.ID != Guid.Empty)
                    return Json(_entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Check Trùng dữ liệu
        #region TAMScanMissReason
        [HttpPost]
        public ActionResult CheckDuplicateMissReason(string code, int id)
        {
            var isDuplicate = false;
            var service = new Cat_TAMScanReasonMissServices();
            var isDuplicateData = service.IsDuplication(code, id);
            if (isDuplicateData)
            {
                isDuplicate = true;
            }
            return Json(isDuplicate);
        }
        #endregion
        #endregion

        #region General
        private List<HRM.Infrastructure.Utilities.FilterAttribute> ExtractAdvanceFilterAttributes(object model)
        {
            List<HRM.Infrastructure.Utilities.FilterAttribute> list = new List<HRM.Infrastructure.Utilities.FilterAttribute>();
            if (model == null)
                return list;

            PropertyInfo[] propertyInfos = model.GetType().GetProperties();
            List<PropertyInfo> lstPropertyInfo = propertyInfos.ToList();

            foreach (PropertyInfo _profertyInfo in lstPropertyInfo)
            {
                HRM.Infrastructure.Utilities.FilterAttribute attribute = new HRM.Infrastructure.Utilities.FilterAttribute()
                {
                    Member = _profertyInfo.Name,
                    MemberType = _profertyInfo.PropertyType,
                    Value2 = model.GetPropertyValue(_profertyInfo.Name)

                };
                if (_profertyInfo.PropertyType.Name == "List`1")
                {
                    attribute.MemberType = typeof(object);
                    var lstObj = (model.GetPropertyValue(_profertyInfo.Name) as IList);
                    object result = null;
                    if (lstObj != null)
                        result = string.Join(",", lstObj.OfType<object>().Select(x => x.ToString()).ToArray());
                    attribute.Value2 = result;
                }
                else if (_profertyInfo.PropertyType == typeof(DateTime))
                {
                    attribute.MemberType = typeof(DateTime);
                    if (attribute.Value2 != null && attribute.Value2.ToString() == DateTime.MinValue.ToString())
                    {
                        attribute.Value2 = null;
                    }
                }

                list.Add(attribute);
            }
            return list;
        }
        private List<SortAttribute> ExtractSortAttributes(DataSourceRequest request)
        {
            List<SortAttribute> list = new List<SortAttribute>();
            if (request.Sorts == null)
                return list;
            foreach (var sort in request.Sorts)
            {
                SortAttribute attribute = new SortAttribute()
                {
                    Member = sort.Member,
                    SortDirection = sort.SortDirection
                };
                list.Add(attribute);
            }
            return list;
        }
        private List<HRM.Infrastructure.Utilities.FilterAttribute> ExtractFilterAttributes(DataSourceRequest request)
        {
            List<HRM.Infrastructure.Utilities.FilterAttribute> list = new List<HRM.Infrastructure.Utilities.FilterAttribute>();
            if (request.Filters == null)
                return list;
            foreach (Kendo.Mvc.FilterDescriptor filter in request.Filters)
            {
                HRM.Infrastructure.Utilities.FilterAttribute attribute = new HRM.Infrastructure.Utilities.FilterAttribute()
                {
                    Member = filter.Member,
                    MemberType = filter.MemberType,
                };
                switch (filter.Operator)
                {
                    case Kendo.Mvc.FilterOperator.IsEqualTo:
                        attribute.Operator = FILTEROPERATOR.Equals;
                        break;
                    case Kendo.Mvc.FilterOperator.Contains:
                        attribute.Operator = FILTEROPERATOR.Contains;
                        break;
                    case Kendo.Mvc.FilterOperator.StartsWith:
                        attribute.Operator = FILTEROPERATOR.StartWith;
                        break;
                    case Kendo.Mvc.FilterOperator.EndsWith:
                        attribute.Operator = FILTEROPERATOR.EndWith;
                        break;
                }
                list.Add(attribute);
            }
            return list;
        }
        #endregion

        #region PayrollGroup

        [HttpPost]
        public ActionResult GetPayrollGroupList([DataSourceRequest] DataSourceRequest request, Cat_PayrollGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_PayrollGroupModel, Cat_PayrollGroupEntity, Cat_PayrollGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_payrollGroup);
        }



        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllPayrollGroupList([DataSourceRequest] DataSourceRequest request, Cat_PayrollGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_PayrollGroupEntity, Cat_PayrollGroupModel, Cat_PayrollGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_payrollGroup);
        }



        public ActionResult ExportPayrollGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PayrollGroupEntity, Cat_PayrollGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PayrollGroupByIds);
        }
        public JsonResult GetPayrollGroupOrd(string text)
        {
            if (text == null || text == " ")
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_PayrollGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_PayrollGroup_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<Cat_PayrollGroupMultiModel> listModel = listEntity.Translate<Cat_PayrollGroupMultiModel>();
                listModel = listModel.OrderBy(s => s.PayrollGroupName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiPayrollGroup(string text)
        {
            return GetDataForControl<Cat_PayrollGroupMultiEntity, Cat_PayrollGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_PayrollGroup_multi);
        }

        #endregion

        #region AccountType
        [HttpPost]
        public ActionResult GetAccountTypeList([DataSourceRequest] DataSourceRequest request, Cat_AccountTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_AccountTypeModel, Cat_AccountTypeEntity, Cat_AccountTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AccountType);
        }
        /// [Phuoc.Le] - Xuất danh sách dữ liệu choTrợ Cấp (Cat_AccountType) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllAccountTypeList([DataSourceRequest] DataSourceRequest request, Cat_AccountTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_AccountTypeEntity, Cat_AccountTypeModel, Cat_AccountTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AccountType);
        }

        /// [Phuoc.Le] - Xuất các dòng dữ liệu được chọn của  Trợ Cấp (Cat_AccountType) theo điều kiện tìm kiếm

        public ActionResult ExportAccountTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_AccountTypeEntity, Cat_AccountTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_AccountTypeByIds);
        }

        public JsonResult GetMultiAccountType(string text)
        {
            return GetDataForControl<Cat_AccountTypeMultiModel, Cat_AccountTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_AccountType_Multi);
        }



        #endregion

        #region Cat_UnusualAllowanceCfg

        public JsonResult GetMultiCfgByUnusualAllowanceGroup(string text, string type)
        {
            string status = string.Empty;
            List<object> para = new List<object>();
            para.AddRange(new object[2]);
            para[0] = text;
            para[1] = type;
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(para, ConstantSql.hrm_cat_sp_get_CfgByUnusualAllowanceGroup_multi, UserLogin, ref status);
            result = result.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUnusualAllowanceCfgList([DataSourceRequest] DataSourceRequest request, CatUnusualAllowanceCfgSearchModel model)
        {
            return GetListDataAndReturn<Cat_UnusualAllowanceCfgModel, Cat_UnusualAllowanceCfgEntity, CatUnusualAllowanceCfgSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfgList);
        }

        public JsonResult UnusualAllowanceCfg_multi(string text)
        {
            Guid _tmp = Guid.Empty;
            if (text == null || text == " " || Guid.TryParse(text, out _tmp) == true)
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_UnusualAllowanceCfgMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                List<Cat_UnusualAllowanceCfgMuitlModel> listModel = listEntity.Translate<Cat_UnusualAllowanceCfgMuitlModel>();
                listModel = listModel.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnusualAllowanceCfg_multi_ForPerformanceAllowance(string text)
        {
            Guid _tmp = Guid.Empty;
            if (text == null || text == " " || Guid.TryParse(text, out _tmp) == true)
                text = string.Empty;
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_UnusualAllowanceCfgMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg_multi_ForPerformanceAllowance, UserLogin, ref status);
            if (listEntity != null)
            {
                List<Cat_UnusualAllowanceCfgMuitlModel> listModel = listEntity.Translate<Cat_UnusualAllowanceCfgMuitlModel>();
                listModel = listModel.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
                return Json(listModel, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult Get_UnusualCfg_Multi_CodeName(string text)
        //{
        //    if (text == null || text == " ")
        //        text = string.Empty;
        //    var service = new BaseService();
        //    string status = string.Empty;
        //    var listEntity = service.GetData<Cat_UnusualAllowanceCfgMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg_multi, UserLogin, ref status);
        //    if (listEntity != null)
        //    {
        //        List<Cat_UnusualAllowanceCfgMuitlModel> listModel = listEntity.Translate<Cat_UnusualAllowanceCfgMuitlModel>();
        //        listModel = listModel.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
        //        //listModel.ForEach(m=>m.Code+" - "+m.UnusualAllowanceCfgNameCode)
        //        return Json(listModel, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult CatUnusualAllowanceCfg_multi(string text)
        {
            return GetDataForControl<Cat_UnusualAllowanceCfgMuitlModel, Cat_UnusualAllowanceCfgMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg_multi);
            //if (text == null || text == " ")
            //    text = string.Empty;
            //var service = new BaseService();
            //string status = string.Empty;
            //var listEntity = service.GetData<Cat_UnusualAllowanceCfgMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg_multi, ref status);
            //if (listEntity != null)
            //{
            //    List<Cat_UnusualAllowanceCfgMuitlModel> listModel = listEntity.Translate<Cat_UnusualAllowanceCfgMuitlModel>();
            //    listModel = listModel.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            //    return Json(listModel, JsonRequestBehavior.AllowGet);
            //}
            //return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportAllUnusualAllowanceCfgList([DataSourceRequest] DataSourceRequest request, CatUnusualAllowanceCfgSearchModel model)
        {
            request.PageSize = int.MaxValue - 1;
            return ExportAllAndReturn<Cat_UnusualAllowanceCfgEntity, Cat_UnusualAllowanceCfgModel, CatUnusualAllowanceCfgSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg);
        }

        public ActionResult ExportUnusualAllowanceCfgSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_UnusualAllowanceCfgEntity, Cat_UnusualAllowanceCfgModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfgIds);
        }


        public JsonResult GetMultiUnusualAllowanceCfg(string text, string type, string cfgType)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(type != string.Empty ? type : null);
            objs.Add(cfgType != string.Empty ? cfgType : null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status);
            result = result.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiUnusualAllowanceCfgByUnusualAllowanceGroup(string text, string eDType, string type, string unusualAllowanceGroup)
        {
            string status = string.Empty;
            List<object> listPara = new List<object>();
            listPara.AddRange(new object[7]);
            if (!string.IsNullOrEmpty(text))
            {
                listPara[0] = text;
                listPara[1] = text;
            }
            if (!string.IsNullOrEmpty(eDType))
            {
                listPara[2] = eDType;
            }
            if (!string.IsNullOrEmpty(type))
            {
                listPara[3] = type;
            }
            if (!string.IsNullOrEmpty(unusualAllowanceGroup))
            {
                listPara[4] = unusualAllowanceGroup;
            }
            listPara[5] = 1;
            listPara[6] = 500;

            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(listPara, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfgByGroup_multi, UserLogin, ref status);
            result = result.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiUnusualAllowanceCfgUnusualAllowanceGroup(string text, string type, string typeUnusual)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(type != string.Empty ? type : null);
            objs.Add(typeUnusual != string.Empty ? typeUnusual : null);
            objs.Add(UnusualAllowanceGroup.E_UNUSUAL.ToString());
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_CfgUnusualAllowanceGroup_multi, UserLogin, ref status);
            result = result.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiUnusualAllowanceCfgByUnusualAllowance(string text, string type)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(type != string.Empty ? type : null);
            objs.Add(EnumDropDown.UnusualEDType.E_UNUSUALALLOWANCE.ToString());
            objs.Add(UnusualAllowanceGroup.E_UNUSUAL.ToString());
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_CfgUnusualAllowanceGroup_multi, UserLogin, ref status);
            result = result.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Chỉ lấy loại PC là bất thường (E_UNUSUALALLOWANCE)
        public JsonResult GetMultiUnusualAllowanceEDCfg(string text, string type)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(null);
            objs.Add(null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //loc theo khoang nhan hay khoang tru
        public JsonResult GetMultiUnusualAllowanceEDCfgByEDType(string text, string type)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(type);
            objs.Add(null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Chỉ lấy loại PC là thường xuyên (E_ALLOWANCE)
        public JsonResult GetMultiUnusualAllowanceUnCfg(string text, string type)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(type);
            objs.Add(HRM.Infrastructure.Utilities.EnumDropDown.UnusualEDType.E_ALLOWANCE.ToString());
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualAllowanceEDCfg, UserLogin, ref status);
            result = result.OrderBy(s => s.UnusualAllowanceCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiUnusualAllowanceCfgPaidLeave(string text, string type)
        {
            //if (text == string.Empty || text == null)
            //{
            //    text = "Paid";
            //}
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(null);
            objs.Add(null);
            objs.Add(type);
            objs.Add(null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status).Where(m => m.Code == text).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiUnusualAllowanceCfgBonusEvaluation(string text, string type)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(null);
            objs.Add(null);
            objs.Add(type);
            objs.Add(null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status).Where(m => m.Code == text).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiUnusualCfgChildCare(string text)
        {
            return GetDataForControl<Cat_UnusualAllowanceCfgMuitlModel, Cat_UnusualAllowanceCfgMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnuCfgbyChild);
        }

        public JsonResult GetMultiUnusualEDTypeEvent(string text)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(true);
            objs.Add(UnusualAllowanceGroup.E_EVENT.ToString());
            var result = baseService.GetData<Cat_UnusualAllowanceCfgMuitlModel>(objs, ConstantSql.hrm_cat_sp_get_UnusualEDTypeCfgGroup_multi, UserLogin, ref status).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_Budget
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_Budget
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBudgetList([DataSourceRequest] DataSourceRequest request, Cat_BudgetSearchModel model)
        {
            return GetListDataAndReturn<Cat_BudgetModel, Cat_BudgetEntity, Cat_BudgetSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Budget);
        }

        /// [Phuoc.Le] - Xuất danh sách dữ liệu choTrợ Cấp (Cat_Budget) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllCatBudgetList([DataSourceRequest] DataSourceRequest request, Cat_BudgetSearchModel model)
        {
            return ExportAllAndReturn<Cat_BudgetEntity, Cat_BudgetModel, Cat_BudgetSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Budget);
        }

        /// [Phuoc.Le] - Xuất các dòng dữ liệu được chọn của  Trợ Cấp (Cat_Budget) theo điều kiện tìm kiếm

        public ActionResult ExportCatBudgetSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_BudgetEntity, Cat_BudgetModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_BudgetByIds);
        }

        public JsonResult GetMultiBudget(string text)
        {
            return GetDataForControl<Cat_BudgetMultiModel, Cat_BudgetMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Budget_Multi);
        }



        #endregion

        #region Cat_YouthUnionPosition
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_YouthUnionPosition
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetYouthUnionPositionList([DataSourceRequest] DataSourceRequest request, Cat_YouthUnionPositionSearchModel model)
        {
            return GetListDataAndReturn<Cat_YouthUnionPositionModel, Cat_YouthUnionPositionEntity, Cat_YouthUnionPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_YouthUnionPosition);
        }

        public JsonResult GetMultiYouthUnionPosition(string text)
        {
            return GetDataForControl<Cat_YouthUnionPositionModel, Cat_YouthUnionPositionEntity>(text, ConstantSql.hrm_cat_sp_get_YouthUnionPosition_Multi);
        }

        public ActionResult ExportYouthUnionPositionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_YouthUnionPositionEntity, Cat_YouthUnionPositionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_YouthUnionPositionByIds);
        }

        // [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllYouthUnionPositionList([DataSourceRequest] DataSourceRequest request, Cat_YouthUnionPositionSearchModel model)
        {
            return ExportAllAndReturn<Cat_YouthUnionPositionEntity, Cat_YouthUnionPositionModel, Cat_YouthUnionPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_YouthUnionPosition);
        }
        #endregion

        #region Cat_CommunistPartyPosition
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_CommunistPartyPosition
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCommunistPartyPositionList([DataSourceRequest] DataSourceRequest request, Cat_CommunistPartyPositionSearchModel model)
        {
            return GetListDataAndReturn<Cat_CommunistPartyPositionModel, Cat_CommunistPartyPositionEntity, Cat_CommunistPartyPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CommunistPartyPosition);
        }

        public ActionResult ExportCatCommunistPartyPositionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CommunistPartyPositionEntity, Cat_CommunistPartyPositionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_CommunistPartyPositionByIds);
        }

        public ActionResult ExportAllCommunistPartyPositionList([DataSourceRequest] DataSourceRequest request, Cat_CommunistPartyPositionSearchModel model)
        {
            return ExportAllAndReturn<Cat_CommunistPartyPositionEntity, Cat_CommunistPartyPositionModel, Cat_CommunistPartyPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CommunistPartyPosition);
        }
        //public JsonResult GetMultiCommunistPartyPosition(string text)
        //{
        //    return GetDataForControl<Cat_CommunistPartyPositionModel, Cat_CommunistPartyPositionEntity>(text, ConstantSql.hrm_cat_sp_get_CommunistPartyPosition_Multi);
        //}
        /// <summary>
        /// [Tho.Bui]: Get mutiselect CommunistPartyPosition
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public JsonResult GetMultiCommunistPartyPosition(string text)
        {
            return GetDataForControl<Cat_CommunistPartyPositionModel, Cat_CommunistPartyPositionEntity>(text, ConstantSql.hrm_cat_sp_get_CommunistPartyPosition_multi);
        }
        #endregion

        #region Cat_WoundedSoldierTypes
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_WoundedSoldierTypes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetWoundedSoldierTypesList([DataSourceRequest] DataSourceRequest request, Cat_WoundedSoldierTypesSearchModel model)
        {
            return GetListDataAndReturn<Cat_WoundedSoldierTypesModel, Cat_WoundedSoldierTypesEntity, Cat_WoundedSoldierTypesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_WoundedSoldierTypes);
        }

        public JsonResult GetMultiWoundedSoldierTypes(string text)
        {
            return GetDataForControl<Cat_WoundedSoldierTypesModel, Cat_WoundedSoldierTypesEntity>(text, ConstantSql.hrm_cat_sp_get_WoundedSoldierTypes_Multi);
        }

        public ActionResult ExportCatWoundedSoldierTypesSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_WoundedSoldierTypesEntity, Cat_WoundedSoldierTypesModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_WoundedSoldierTypesByIds);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllWoundedSoldierTypesList([DataSourceRequest] DataSourceRequest request, Cat_WoundedSoldierTypesSearchModel model)
        {
            return ExportAllAndReturn<Cat_WoundedSoldierTypesEntity, Cat_WoundedSoldierTypesModel, Cat_WoundedSoldierTypesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_WoundedSoldierTypes);
        }
        #endregion

        #region Cat_PoliticalLevel
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_PoliticalLevel
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPoliticalLevelList([DataSourceRequest] DataSourceRequest request, Cat_PoliticalLevelSearchModel model)
        {
            return GetListDataAndReturn<Cat_PoliticalLevelModel, Cat_PoliticalLevelEntity, Cat_PoliticalLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PoliticalLevel);
        }

        public JsonResult GetMultiPoliticalLevel(string text)
        {
            return GetDataForControl<Cat_PoliticalLevelModel, Cat_PoliticalLevelEntity>(text, ConstantSql.hrm_cat_sp_get_PoliticalLevel_Multi);
        }

        public ActionResult ExportCatPoliticalLevelSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PoliticalLevelEntity, Cat_PoliticalLevelModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PoliticalLevelByIds);
        }

        public ActionResult ExportAllPoliticalLevelList([DataSourceRequest] DataSourceRequest request, Cat_PoliticalLevelSearchModel model)
        {
            return ExportAllAndReturn<Cat_PoliticalLevelEntity, Cat_PoliticalLevelModel, Cat_PoliticalLevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PoliticalLevel);
        }

        #endregion

        #region Cat_ArmedForceTypes
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_ArmedForceTypes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetArmedForceTypesList([DataSourceRequest] DataSourceRequest request, Cat_ArmedForceTypesSearchModel model)
        {
            return GetListDataAndReturn<Cat_ArmedForceTypesModel, Cat_ArmedForceTypesEntity, Cat_ArmedForceTypesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ArmedForceTypes);
        }

        public JsonResult GetMultiArmedForceTypes(string text)
        {
            return GetDataForControl<Cat_ArmedForceTypesModel, Cat_ArmedForceTypesEntity>(text, ConstantSql.hrm_cat_sp_get_ArmedForceTypes_Multi);
        }

        public ActionResult ExportCatArmedForceTypesSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ArmedForceTypesEntity, Cat_ArmedForceTypesModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ArmedForceTypesByIds);
        }

        public ActionResult ExportAllArmedForceTypesList([DataSourceRequest] DataSourceRequest request, Cat_ArmedForceTypesSearchModel model)
        {
            return ExportAllAndReturn<Cat_ArmedForceTypesEntity, Cat_ArmedForceTypesModel, Cat_ArmedForceTypesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ArmedForceTypes);
        }
        #endregion

        #region Cat_TradeUnionistPosition
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_TradeUnionistPosition
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTradeUnionistPositionList([DataSourceRequest] DataSourceRequest request, Cat_TradeUnionistPositionSearchModel model)
        {
            return GetListDataAndReturn<Cat_TradeUnionistPositionModel, Cat_TradeUnionistPositionEntity, Cat_TradeUnionistPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TradeUnionistPosition);
        }

        public JsonResult GetMultiTradeUnionistPosition(string text)
        {
            return GetDataForControl<Cat_TradeUnionistPositionModel, Cat_TradeUnionistPositionEntity>(text, ConstantSql.hrm_cat_sp_get_TradeUnionistPosition_Multi);
        }

        public ActionResult ExportCatTradeUnionistPositionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_TradeUnionistPositionEntity, Cat_TradeUnionistPositionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_TradeUnionistPositionByIds);
        }

        public ActionResult ExportAllTradeUnionistPositionList([DataSourceRequest] DataSourceRequest request, Cat_TradeUnionistPositionSearchModel model)
        {
            return ExportAllAndReturn<Cat_TradeUnionistPositionEntity, Cat_TradeUnionistPositionModel, Cat_TradeUnionistPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_TradeUnionistPosition);
        }

        #endregion

        #region Cat_SelfDefenceMilitiaPosition
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_SelfDefenceMilitiaPosition
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSelfDefenceMilitiaPositionList([DataSourceRequest] DataSourceRequest request, Cat_SelfDefenceMilitiaPositionSearchModel model)
        {
            return GetListDataAndReturn<Cat_SelfDefenceMilitiaPositionModel, Cat_SelfDefenceMilitiaPositionEntity, Cat_SelfDefenceMilitiaPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SelfDefenceMilitiaPosition);
        }

        public JsonResult GetMultiSelfDefenceMilitiaPosition(string text)
        {
            return GetDataForControl<Cat_SelfDefenceMilitiaPositionModel, Cat_SelfDefenceMilitiaPositionEntity>(text, ConstantSql.hrm_cat_sp_get_SelfDefenceMilitiaPosition_Multi);
        }

        public ActionResult ExportCatSelfDefenceMilitiaPositionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SelfDefenceMilitiaPositionEntity, Cat_SelfDefenceMilitiaPositionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SelfDefenceMilitiaPositionByIds);
        }

        public ActionResult ExportAllSelfDefenceMilitiaPositionList([DataSourceRequest] DataSourceRequest request, Cat_SelfDefenceMilitiaPositionSearchModel model)
        {
            return ExportAllAndReturn<Cat_SelfDefenceMilitiaPositionEntity, Cat_SelfDefenceMilitiaPositionModel, Cat_SelfDefenceMilitiaPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SelfDefenceMilitiaPosition);
        }

        #endregion

        #region Cat_VeteranUnionPosition
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_VeteranUnionPosition
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetVeteranUnionPositionList([DataSourceRequest] DataSourceRequest request, Cat_VeteranUnionPositionSearchModel model)
        {
            return GetListDataAndReturn<Cat_VeteranUnionPositionModel, Cat_VeteranUnionPositionEntity, Cat_VeteranUnionPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_VeteranUnionPosition);
        }

        public JsonResult GetMultiVeteranUnionPosition(string text)
        {
            return GetDataForControl<Cat_VeteranUnionPositionModel, Cat_VeteranUnionPositionEntity>(text, ConstantSql.hrm_cat_sp_get_VeteranUnionPosition_Multi);
        }

        public ActionResult ExportCatVeteranUnionPositionSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_VeteranUnionPositionEntity, Cat_VeteranUnionPositionModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_VeteranUnionPositionByIds);
        }

        public ActionResult ExportAllVeteranUnionPositionList([DataSourceRequest] DataSourceRequest request, Cat_VeteranUnionPositionSearchModel model)
        {
            return ExportAllAndReturn<Cat_VeteranUnionPositionEntity, Cat_VeteranUnionPositionModel, Cat_VeteranUnionPositionSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_VeteranUnionPosition);
        }

        #endregion

        #region Cat_SalaryRank
        //Son.Vo - 20160912 - 0072256
        [HttpPost]
        public ActionResult GetDataOfSalaryRankEvaluContract(Guid? ID)
        {
            if (ID != null && ID != Guid.Empty)
            {
                var service = new Cat_SalaryRankServices();
                var entity = service.GetDataOfSalaryRankEvaluContract(ID.Value);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        //Son.Vo - 20160912 - 0072256
        [HttpPost]
        public ActionResult GetDataOfSalaryCreateMultiContractContract(Guid? salaryrankID, string strContractID)
        {
            if (salaryrankID != null && salaryrankID != Guid.Empty && strContractID != null)
            {
                var contractID = strContractID.Split(',').Select(x => Guid.Parse(x)).FirstOrDefault();
                var contractservice = new Hre_ContractServices();
                var entity = contractservice.GetDataOfSalaryRankCreateMultiContract(salaryrankID.Value, contractID);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// [Tho.Bui] - Lấy danh sách dữ liệu bảng Cat_SalaryRank (Cat_SalaryRank)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSalaryRankList([DataSourceRequest] DataSourceRequest request, Cat_SalaryRankSearchModel model)
        {
            return GetListDataAndReturn<Cat_SalaryRankModel, Cat_SalaryRankEntity, Cat_SalaryRankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryRank);
        }
        /// [Tho.Bui] - Xuất các dòng dữ liệu được chọn của mã lương (Cat_SalaryClass) theo điều kiện tìm kiếm
        public ActionResult ExportSalaryRankSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SalaryRankEntity, Cat_SalaryRankModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryRankByIds);
        }
        /// [Tho.Bui] - Xuất danh sách dữ liệu cho mã lương (Cat_SalaryRank) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAlSalaryRankList([DataSourceRequest] DataSourceRequest request, Cat_SalaryRankSearchModel model)
        {
            return ExportAllAndReturn<Cat_SalaryRankEntity, Cat_SalaryRankModel, Cat_SalaryRankSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryRank);
        }

        public JsonResult GetMultiSalaryRankBySalaryClassIDAndDateOfEffect(string text, Guid SalaryClassID, DateTime? DateOfEffect)
        {
            if (DateOfEffect != null)
            {
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(text);
                objs.Add(Common.DotNetToOracle(SalaryClassID.ToString()));
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                      .Where(s => (s.DateOfEffect == null || s.DateOfEffect < DateOfEffect) && (s.DateEnd >= DateOfEffect || s.DateEnd == null) && s.SalaryClassID == SalaryClassID).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(text);
                objs.Add(Common.DotNetToOracle(SalaryClassID.ToString()));
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankMultiEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status).OrderBy(s => s.SalaryRankName).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetMultiSalaryRank(string text, string SalaryClassID)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(Common.DotNetToOracle(SalaryClassID));
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_SalaryRankMultiEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status).OrderBy(s => s.SalaryRankName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiSalaryRankInsurance(string text, string SalaryClassID)
        {
            string status = string.Empty;
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(Common.DotNetToOracle(SalaryClassID));
            objs.Add(null);
            objs.Add(null);
            objs.Add(null);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status).Where(m => m.IsInsurance == true).OrderBy(s => s.SalaryRankName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiSalaryRankAndRankDetail(string text)
        {
            return GetDataForControl<Cat_SalaryRankMultiEntity, Cat_SalaryRankMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalaryRank_Multi);
        }

        public JsonResult GetMultiSalaryRankClassList(string text)
        {
            return GetDataForControl<Cat_SalaryClassMultiEntity, Cat_SalaryClassMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalaryRankClass_Multi);
        }

        [HttpPost]
        public ActionResult GetDataOfSalaryRank(Guid? ID)
        {
            if (ID != null)
            {
                var service = new ActionService(UserLogin, LanguageCode);
                string status = "";
                var entity = service.GetByIdUseStore<Cat_SalaryRankEntity>(ID.Value, ConstantSql.hrm_cat_sp_get_SalaryRankById, ref status);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDataOfSalaryRankUpdateFileCandidate(Guid? ID, Guid? contractTypeID)
        {
            if (ID != null)
            {
                var service = new ActionService(UserLogin, LanguageCode);
                string status = "";
                var entity = service.GetByIdUseStore<Cat_SalaryRankEntity>(ID.Value, ConstantSql.hrm_cat_sp_get_SalaryRankById, ref status);
                if (contractTypeID != null)
                {
                    var contractTypeServices = new Cat_ContractTypeServices();
                    var contractType = contractTypeServices.GetDataContractTypeByID(contractTypeID.Value);
                    if (contractType != null)
                    {
                        //Son.Vo - 20160908 - 0072341
                        if (contractType.Type == EnumDropDown.TypeContract.E_PROBATION.ToString())
                        {
                            entity.BasicSalary = entity.ProbationSalary;
                        }
                        else
                        {
                            entity.BasicSalary = entity.SalaryStandard;
                        }
                    }
                }
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSalaryRankBySalaryClassID(Guid? SalaryClassID)
        {
            if (SalaryClassID != null)
            {
                var service = new ActionService(UserLogin, LanguageCode);
                string status = string.Empty;
                var result = service.GetData<Cat_SalaryRankMultiEntity>(Common.DotNetToOracle(SalaryClassID.ToString()), ConstantSql.hrm_cat_sp_get_SalaryRankBySalaryClassId, ref status);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetSalaryRankBySalaryClassIDAndDateEffect(Guid? SalaryClassID, DateTime? dateEffect)
        {
            if (SalaryClassID != null && dateEffect != null)
            {
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                    .Where(s => (s.DateOfEffect == null || s.DateOfEffect < dateEffect) && (s.DateEnd >= dateEffect || s.DateEnd == null) && s.SalaryClassID == SalaryClassID).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (SalaryClassID != null && dateEffect == null)
            {
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                    .Where(s => s.SalaryClassID == SalaryClassID).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (SalaryClassID == null && dateEffect != null)
            {
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                    .Where(s => s.DateOfEffect < dateEffect && (s.DateEnd >= dateEffect || s.DateEnd == null)).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ComputeSalaryRankBasicSalary(Guid? ProfileID, Guid? SalaryClassID, DateTime? dateEffect)
        {
            if (!ProfileID.HasValue || !SalaryClassID.HasValue || !dateEffect.HasValue)
            {
                return Json(new ResultsObject() { Success = false, Messenger = string.Empty });
            }

            Cat_SalaryRankServices services = new Cat_SalaryRankServices();
            return Json(new ResultsObject() { Success = true, Data = services.ComputeSalaryRank(ProfileID.Value, SalaryClassID.Value, dateEffect.Value) });
        }

        // Son.Vo - theo task 0052657
        [HttpPost]
        public ActionResult GetSalaryRankByDate(DateTime? Datefrom)
        {
            if (Datefrom != null)
            {
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                    .Where(s => s.DateOfEffect < Datefrom && (s.DateEnd >= Datefrom || s.DateEnd == null)).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSalaryRankForUpdateEvaContractByDate(DateTime? Datecheck)
        {
            if (Datecheck != null)
            {
                DateTime Datefrom = Datecheck.Value.AddDays(1);
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                    .Where(s => s.DateOfEffect < Datefrom && (s.DateEnd >= Datefrom || s.DateEnd == null)).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        //Son.Vo - 20160531 - 0068283 - logic lấy bl chi tiết mới 1
        // Son.Vo - theo task 0052657
        [HttpPost]
        public ActionResult GetSalaryRankForEvaContract(Guid? salaryClassID, DateTime? datecheck)
        {
            if (salaryClassID != null && datecheck != null)
            {
                datecheck = datecheck.Value.Date;
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(Int32.MaxValue - 1);
                var result = baseService.GetData<Cat_SalaryRankEntity>(objs, ConstantSql.hrm_cat_sp_get_SalaryRank, UserLogin, ref status)
                .Where(s => s.DateEnd >= datecheck.Value.AddDays(1) && datecheck.Value.AddDays(1) >= s.DateOfEffect && s.SalaryClassID == salaryClassID).OrderByDescending(s => s.DateOfEffect).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Cat_Budget
        /// <summary>
        /// [Kiet.Chung] - Lấy danh sách dữ liệu bảng Cat_NameEntity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetNameEntityList([DataSourceRequest] DataSourceRequest request, Cat_NameEntitySearchModel model)
        {
            string status = string.Empty;
            List<object> lstModel = new List<object>();
            lstModel.AddRange(new object[4]);

            lstModel[0] = model.NameEntityName;
            lstModel[1] = model.NameEntityType;
            lstModel[2] = 1;
            lstModel[3] = int.MaxValue - 1;
            ActionService actionService = new ActionService(UserLogin, LanguageCode);
            var lstResult = actionService.GetData<Cat_NameEntityEntity>(lstModel, ConstantSql.hrm_cat_sp_get_NameEntity, ref status);
            //Quyen.Quach 04/11/2017 0089364
            if (model.IsExport)
            {
                string message = ExportService.Export(Guid.Empty, lstResult, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','), null);
                return Json(message);
            }
            return Json(lstResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            //return GetListDataAndReturn<Cat_NameEntityModel, Cat_NameEntityEntity, Cat_NameEntitySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntity);
        }

        public ActionResult ExportAllEntityByKPI([DataSourceRequest] DataSourceRequest request, Cat_NameEntityByKPISearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, Cat_NameEntityModel, Cat_NameEntityByKPISearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntityByKPI);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllNameEntityList([DataSourceRequest] DataSourceRequest request, Cat_NameEntitySearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_NameEntitySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntity);
        }
        public ActionResult ExportAllNameEntityListNew([DataSourceRequest] DataSourceRequest request, Cat_NameEntitySearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_NameEntitySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntity);
        }

        public ActionResult ExportNameEntitySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelGeneralByIds);
        }

        #endregion
        #region Cat_KPIGroup
        [HttpPost]
        public ActionResult GetNameEntityByKPI([DataSourceRequest] DataSourceRequest request, Eva_KPIGroupSearchModel model)
        {
            return GetListDataAndReturn<Eva_KPIGroupModel, Eva_KPIGroupEntity, Eva_KPIGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntityByKPI);
        }
        public ActionResult ExportAllKPIGroup([DataSourceRequest] DataSourceRequest request, Eva_KPIGroupSearchModel model)
        {
            return ExportAllAndReturn<Eva_KPIGroupModel, Eva_KPIGroupEntity, Eva_KPIGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntityByKPI);
        }
        public ActionResult ExportKPIGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Eva_KPIGroupEntity, Eva_KPIGroupModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_KPIGroupByIds);
        }

        #endregion

        #region Fac_FacilityType
        [HttpPost]
        public ActionResult GetNameEntityListByThreeField([DataSourceRequest] DataSourceRequest request, Cat_NameEntitySearchCodeModel model)
        {
            return GetListDataAndReturn<Cat_NameEntityModel, Cat_NameEntityEntity, Cat_NameEntitySearchCodeModel>(request, model, ConstantSql.hrm_cat_sp_get_NameEntityByThreeField);
        }
        #endregion

        #region Export Enum Element From strFormular



        //[28/10/2016][bang.nguyen][new func][74128]
        //Thêm chức Xem và xuất các Enum hổ trợ đặc công thức tại mỗi Textfield công thức (Lương)
        public ActionResult ExportEnumFromFormular(string strFormula)
        {
            ExportService exportServices = new ExportService();
            DataTable table = new DataTable();
            table.Columns.Add("EnumCode");
            table.Columns.Add("EnumName");
            if (!string.IsNullOrEmpty(strFormula))
            {

                List<string> ListFormula = new List<string>();
                strFormula = strFormula.Replace("\n", "").Replace("\t", "").Trim();
                //Các phần tử tính lương tách ra từ 1 chuỗi công thức
                ListFormula.AddRange(Common.ParseFormulaToList(strFormula).Where(m => m.IndexOf('[') != -1 && m.IndexOf(']') != -1).ToList());
                foreach (var enumElement in ListFormula)
                {
                    var keyEnum = enumElement.Replace("[", "").Replace("]", "").ToString();
                    DataRow row = table.NewRow();
                    row["EnumCode"] = keyEnum;
                    table.Rows.Add(row);
                }
            }
            if (table.Rows.Count == 0)
            {
                DataRow row = table.NewRow();
                row["EnumCode"] = string.Empty;
                row["EnumName"] = string.Empty;
                table.Rows.Add(row);
            }
            string filename = "EnumFromFormular";
            string fileExt = "xlsx";
            string fileSuffix = DateTime.Now.ToString("HHmmss");
            string outputFileName = filename + fileSuffix + "." + fileExt;
            string dirpath = Common.GetPath(Common.DownloadURL);
            string outputPath = dirpath + outputFileName;
            outputPath = outputPath.Replace("/", "\\");
            string hostPath = ConstantPathWeb.Hrm_Hre_Service + Common.DownloadURL;
            string downloadPath = hostPath + outputFileName;
            var fullPath = exportServices.ExportNormal(outputPath, table);
            return Json(NotificationType.Success.ToString() + "," + downloadPath);
        }

        //[26/07/2017][bang.nguyen][81130][Modify Func]
        /// <summary>
        /// liệt kê all enum có thể đặt trong ô công thức
        /// </summary>
        /// <param name="strFormula">ID control cong thuc controller_action_id</param>
        /// <returns></returns>
        public ActionResult ExportEnumForFormular(string strFormula)
        {

            Dictionary<string, string> listDataEnumForFormular = new Dictionary<string, string>();

            if (strFormula == "Sys_SalConfig_Create_HRM_SAL_ELEMENT_COMPUTE")
            {
                listDataEnumForFormular.Add("Value", "AmountPaid".TranslateString() + " (PayrollTableByProfile.E_AmountPaid)");
            }
            else if (strFormula == "Sys_SalConfig_Create_HRM_SAL_ELEMENT_UNUSUALPAY")
            {
                List<SelectListItem> lstUnusualPayElement = Enum.GetValues(typeof(UnusualPayElement))
                .Cast<UnusualPayElement>()
                .Select(x => new SelectListItem { Value = x.ToString(), Text = EnumDropDown.GetEnumDescription(x) })
                .ToList();

                foreach (var item in lstUnusualPayElement)
                {
                    if (!listDataEnumForFormular.ContainsKey(item.Value))
                    {
                        listDataEnumForFormular.Add(item.Value, item.Text);
                    }
                }

                Cat_ElementServices elementServices = new Cat_ElementServices();
                var listElement = elementServices.GetDataForEntity();
                foreach (var item in listElement)
                {
                    if (!listDataEnumForFormular.ContainsKey(item.ElementCode))
                    {
                        listDataEnumForFormular.Add(item.ElementCode, item.ElementName);
                    }
                }
            }
            else if (strFormula == "Cat_Shop_Cat_ShopInfo_Formular" || strFormula == "Cat_Shop_Cat_ShopInfo_Formular1")
            {
                listDataEnumForFormular.Add(PayrollElement.SAL_COM_TAGET_SHOP.ToString(), "Mục tiêu của cửa hàng");
                listDataEnumForFormular.Add(PayrollElement.SAL_COM_ACTUAL_SHOP.ToString(), "Doanh thu thực đạt của cửa hàng");
                listDataEnumForFormular.Add(PayrollElement.SAL_COM_PRECENT_REVENUE.ToString(), "Phần trăm doanh thu của cửa hàng");
                listDataEnumForFormular.Add(PayrollElement.SAL_COM_COUNT_SHOPMEMBER.ToString(), "Số nhân viên của cửa hàng");
                listDataEnumForFormular.Add(PayrollElement.SAL_COM_COUNT_SL.ToString(), "Số ca trưởng của cửa hàng");
                listDataEnumForFormular.Add(PayrollElement.SAL_COM_RANK.ToString(), "Cấp Bậc Của Cửa Hàng");
            }
            else if (strFormula == "Cat_GradeSalDept_Cat_GradeSalDeptInfo_AverageAmountFormula")
            {
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_DEPTRATE.ToString(), "Hệ số phòng ban");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_COMPANYRATE.ToString(), "Hệ Số Công Ty");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_AMOUNT.ToString(), "Quỹ Lương");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_TOTAL_AMOUNTSALARY.ToString(), "Tổng tiền lương nhân viên");
            }
            else if (strFormula == "Cat_GradeSalDept_Cat_GradeSalDeptInfo_AmountFormula")
            {
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_DEPTRATE.ToString(), "Hệ số phòng ban");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_COMPANYRATE.ToString(), "Hệ Số Công Ty");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_AMOUNT.ToString(), "Quỹ Lương");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_RATE.ToString(), "Hệ số cá nhân");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME1HOURS.ToString(), "Số giờ OT 1");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME2HOURS.ToString(), "Số giờ OT 2");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME3HOURS.ToString(), "Số giờ OT 3");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME4HOURS.ToString(), "Số giờ OT 4");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME5HOURS.ToString(), "Số giờ OT 5");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME6HOURS.ToString(), "Số giờ OT 6");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME1TYPE_CODE.ToString(), "Mã Loại OT 1");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME2TYPE_CODE.ToString(), "Mã Loại OT 2");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME3TYPE_CODE.ToString(), "Mã Loại OT 3");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME4TYPE_CODE.ToString(), "Mã Loại OT 4");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME5TYPE_CODE.ToString(), "Mã Loại OT 5");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME6TYPE_CODE.ToString(), "Mã Loại OT 6");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME1TYPE_RATE.ToString(), "Hệ số OT 1");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME2TYPE_RATE.ToString(), "Hệ số OT 2");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME3TYPE_RATE.ToString(), "Hệ số OT 3");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME4TYPE_RATE.ToString(), "Hệ số OT 4");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME5TYPE_RATE.ToString(), "Hệ số OT 5");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME6TYPE_RATE.ToString(), "Hệ số OT 6");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_PAIDWORKHOURS.ToString(), "Số giờ công thực tế");
                listDataEnumForFormular.Add("ATT_OVERTIME_MaLoaiOT_HOURS", "Tổng giờ OT theo loại OT");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_BASIC_SALARY_ITEM.ToString(), "Lương cơ bản từng nhân viên");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_ISTRANSFER.ToString(), "Là điều động");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_SUPPORTRATE.ToString(), "Hệ số điều chỉnh");
                listDataEnumForFormular.Add("SAL_UNUSUALALLOWANCE_MaLoaiPhuCap_AMOUNT_ITEM", "Tong tien theo loai phu cap theo nhan vien");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_DATE_HIRE.ToString(), "Ngày vào làm");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_DATE_QUIT.ToString(), "Ngày nghỉ việc");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_POSITION_NAME.ToString(), "Tên chức danh");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_POSITION_CODE.ToString(), "Mã chức danh");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.ATT_LATEEARLYMINUTES.ToString(), "Phút trễ sớm");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAL_ACTUALHOURS.ToString(), "Giờ công thực tế");
                listDataEnumForFormular.Add("Cat_GradeSalDeptElement_ElementCode", "Bộ ElementCode từ bảng Cat_GradeSalDeptElement");
            }
            else if (strFormula == "Cat_GradeSalDeptElement_Cat_GradeSalDeptElementInfo_Formula")
            {
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.ATT_TOTAL_PAIDWORKDAYCOUNT_IN_YEAR.ToString(), "Tổng số công thực tế trong năm");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.ATT_STD_DAY.ToString(), "Công Chuẩn");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.ATT_ANNUALLEAVE.ToString(), "Nghỉ phép năm (ngày)");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAL_BASIC_SALARY.ToString(), "Lương Cơ Bản");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.EVA_PERFORMENTCE_POINT.ToString(), "Điểm Đánh Giá");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAL_PRODUCT_AMOUNT.ToString(), "Số tiền lương sản phẩm");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAL_INSURANCE_SALARY.ToString(), "Lương BHXH");
                listDataEnumForFormular.Add("SAL_UNUSUALALLOWANCE_MaLoaiPhuCap_AMOUNT", "Tổng tiền phụ cấp theo loại phụ cấp");
                listDataEnumForFormular.Add("ATT_LEAVEDAY_MaLoaiNgayNghi_DAY", "Tổng ngày nghỉ theo loại nghỉ");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAl_SALARY_DEPARTMENT_DEPTRATE.ToString(), "Hệ số phòng ban");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAl_SALARY_DEPARTMENT_COMPANYRATE.ToString(), "Hệ số công ty");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAl_SALARY_DEPARTMENTITEM_RATE.ToString(), "Hệ số phòng ban chi tiết");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAl_SALARY_DEPARTMENTITEM_OVERTIME_HOURS.ToString(), "Tổng giờ OT phòng ban chi tiết");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_DEPTRATE.ToString(), "Hệ số phòng ban");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_COMPANYRATE.ToString(), "Hệ Số Công Ty");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_AMOUNT.ToString(), "Quỹ Lương");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_BASIC_SALARY_ITEM.ToString(), "Lương cơ bản từng nhân viên");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_RATE.ToString(), "Hệ số cá nhân");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME1HOURS.ToString(), "Số giờ OT 1");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME2HOURS.ToString(), "Số giờ OT 2");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME3HOURS.ToString(), "Số giờ OT 3");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME4HOURS.ToString(), "Số giờ OT 4");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME5HOURS.ToString(), "Số giờ OT 5");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME6HOURS.ToString(), "Số giờ OT 6");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME1TYPE_CODE.ToString(), "Mã Loại OT 1");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME2TYPE_CODE.ToString(), "Mã Loại OT 2");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME3TYPE_CODE.ToString(), "Mã Loại OT 3");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME4TYPE_CODE.ToString(), "Mã Loại OT 4");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME5TYPE_CODE.ToString(), "Mã Loại OT 5");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME6TYPE_CODE.ToString(), "Mã Loại OT 6");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME1TYPE_RATE.ToString(), "Hệ số OT 1");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME2TYPE_RATE.ToString(), "Hệ số OT 2");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME3TYPE_RATE.ToString(), "Hệ số OT 3");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME4TYPE_RATE.ToString(), "Hệ số OT 4");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME5TYPE_RATE.ToString(), "Hệ số OT 5");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_OVERTIME6TYPE_RATE.ToString(), "Hệ số OT 6");
                listDataEnumForFormular.Add(SalaryDepartmentElement.SALARY_DEPARMENT_PAIDWORKHOURS.ToString(), "Số giờ công thực tế");
                listDataEnumForFormular.Add("ATT_OVERTIME_MaLoaiOT_HOURS", "Tổng giờ OT theo loại OT");
                listDataEnumForFormular.Add("SAL_UNUSUALALLOWANCE_MaLoaiPhuCap_AMOUNT_ITEM", "Tong tien theo loai phu cap theo nhan vien");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_DATE_HIRE.ToString(), "Ngày vào làm");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_DATE_QUIT.ToString(), "Ngày nghỉ việc");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_POSITION_NAME.ToString(), "Tên chức danh");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.HRE_POSITION_CODE.ToString(), "Mã chức danh");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.ATT_LATEEARLYMINUTES.ToString(), "Phút trễ sớm");
                listDataEnumForFormular.Add(SalaryDepartmentElementItem.SAL_ACTUALHOURS.ToString(), "Giờ công thực tế");

            }

            string downloadPath = ExportAllEnumForFormular(listDataEnumForFormular);
            return Json(NotificationType.Success.ToString() + "," + downloadPath);
        }

        public string ExportAllEnumForFormular(Dictionary<string, string> listDataEnumForFormular)
        {
            ExportService exportServices = new ExportService();
            DataTable table = new DataTable();
            table.Columns.Add("EnumCode");
            table.Columns.Add("EnumName");

            foreach (var item in listDataEnumForFormular)
            {
                DataRow row = table.NewRow();
                row["EnumCode"] = item.Key;
                if (!string.IsNullOrEmpty(item.Value))
                {
                    row["EnumName"] = item.Value;
                }
                table.Rows.Add(row);
            }

            if (table.Rows.Count == 0)
            {
                DataRow row = table.NewRow();
                row["EnumCode"] = string.Empty;
                row["EnumName"] = string.Empty;
                table.Rows.Add(row);
            }
            string filename = "EnumForFormular";
            string fileExt = "xlsx";
            string fileSuffix = DateTime.Now.ToString("HHmmss");
            string outputFileName = filename + fileSuffix + "." + fileExt;
            string dirpath = Common.GetPath(Common.DownloadURL);
            string outputPath = dirpath + outputFileName;
            outputPath = outputPath.Replace("/", "\\");
            string hostPath = ConstantPathWeb.Hrm_Hre_Service + Common.DownloadURL;
            string downloadPath = hostPath + outputFileName;
            var fullPath = exportServices.ExportNormal(outputPath, table);
            return downloadPath;
        }
        #endregion

        #region Cat_Element

        public ActionResult CheckSupperAminByUser(string userLoginName)
        {
            var result = false;
            if (!string.IsNullOrEmpty(userLoginName))
            {
                if (Common.UserNameSystem == userLoginName)
                {
                    result = true;
                }
            }
            return Json(result);
        }

        //[5/10/2016][bang.nguyen][new func][73803]
        public ActionResult ExportEnumProject()
        {
            ExportService exportServices = new ExportService();
            DataTable table = new DataTable();
            Cat_ElementServices catElementServices = new Cat_ElementServices();
            table = catElementServices.GetEnumSalaryByProject(UserLogin);
            string filename = "CatElementEnumSalary";
            string fileExt = "xlsx";
            string fileSuffix = DateTime.Now.ToString("HHmmss");
            string outputFileName = filename + fileSuffix + "." + fileExt;
            string dirpath = Common.GetPath(Common.DownloadURL);
            string outputPath = dirpath + outputFileName;
            outputPath = outputPath.Replace("/", "\\");
            string hostPath = ConstantPathWeb.Hrm_Hre_Service + Common.DownloadURL;
            string downloadPath = hostPath + outputFileName;
            var fullPath = exportServices.ExportNormal(outputPath, table);
            return Json(NotificationType.Success.ToString() + "," + downloadPath);
        }

        //public JsonResult GetMultiGradePayroll()
        //{
        //    return GetDataForControl<Cat_GradePayrollMultiModel, Cat_GradePayrollMultiEntity>("", ConstantSql.hrm_cat_sp_get_GradePayroll_multi);
        //}

        public JsonResult GetMultiGradePayroll(string text)
        {
            string status = string.Empty;
            BaseService baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(1);
            objs.Add(Int32.MaxValue - 1);
            var result = baseService.GetData<Cat_GradePayrollEntity>(objs, ConstantSql.hrm_cat_sp_get_GradePayroll, UserLogin, ref status).OrderBy(s => s.GradeCfgName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiCatElement(string text)
        {
            return GetDataForControl<CatElementModel, Cat_ElementEntity>(text, ConstantSql.hrm_cat_sp_get_CatElement_multi);
        }

        /// <summary>
        /// [Hien.Nguyen] Kiểm tra xem công thức có hợp lệ hay không
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult CheckFormula(string elementCode, string values)
        {
            if (values.Replace("\n", "").Replace(" ", "") == string.Empty)
            {
                return Json(new { success = false, data = 0 });
            }
            values = values.Replace("\n", "");
            string status = string.Empty;
            var baseService = new BaseService();
            //List phần tử đã tách ra từ công thức
            List<string> ListFormula = Common.ParseFormulaToList(values).Where(m => m.IndexOf('[') != -1 && m.IndexOf(']') != -1).ToList();

            Cat_ElementServices elementServices = new Cat_ElementServices();
            List<CatElementModel> listCat_Element = elementServices.GetDataForEntity().Translate<CatElementModel>();

            #region Kiểm tra xem công thức có giống tên phần tử hay không
            Sal_ComputePayrollServices PayrollServices = new Sal_ComputePayrollServices();

            //Các phần tử tính lương tách ra từ 1 chuỗi công thức         
            if (ListFormula.Any(m => m.Replace("[", "").Replace("]", "") == elementCode))
            {
                return Json(new { success = false, mess = ConstantDisplay.HRM_Sal_Formular_Wrong.TranslateString() + " [" + elementCode + "]" });
            }

            //Kiểm tra công thức bị gọi chéo với nhau
            foreach (var i in ListFormula)
            {
                var _element = listCat_Element.FirstOrDefault(m => m.ElementCode == i.Replace("[", "").Replace("]", ""));
                if (_element.HasValue())
                {
                    //Kiểm tra nếu như phần tử này có tồn tại trong công thức của phần tử con thì ko cho tạo mới
                    if (_element.Formula.IndexOf("[" + elementCode + "]") != -1)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Sal_Formular_Wrong.TranslateString() + " [" + elementCode + "]" });
                    }
                }
            }

            #endregion

            #region Add thêm các phần tử Enum để kiểm tra
            var valuesAsList = Enum.GetValues(typeof(PayrollElement)).Cast<PayrollElement>().ToList();
            foreach (var i in valuesAsList)
            {
                listCat_Element.Add(new CatElementModel() { ElementCode = i.ToString(), Formula = "[" + i.ToString() + "]" });
            }
            #endregion
            #region Add thêm các phần tử Enum nhom để kiểm tra
            //[03/11/2017][bang.nguyen][89526][New Func]
            var valuesAsListGroup = Enum.GetValues(typeof(PayrollElementGroup)).Cast<PayrollElementGroup>().ToList();
            foreach (var i in valuesAsListGroup)
            {
                listCat_Element.Add(new CatElementModel() { ElementCode = i.ToString(), Formula = "[" + i.ToString() + "]" });
            }
            #endregion

            #region Add thêm các phần tử Enum (Bảo Hiểm) để kiểm tra
            //[Tung.Ly 20151027][Modify][59056] : thêm các phần từ bảo hiểm để kiểm tra công thức
            var valuesInsuranceAsList = Enum.GetValues(typeof(InsuranceElement)).Cast<InsuranceElement>().ToList();
            foreach (var i in valuesInsuranceAsList)
            {
                listCat_Element.Add(new CatElementModel() { ElementCode = i.ToString(), Formula = "[" + i.ToString() + "]" });
            }
            #endregion

            #region Add thêm các phần tử là phụ cấp
            var listModel = new List<object>();
            listModel.AddRange(new object[4]);
            listModel[2] = 1;
            listModel[3] = Int32.MaxValue - 1;
            var listUsualAllowance = baseService.GetData<Cat_UsualAllowanceModel>(listModel, ConstantSql.hrm_cat_sp_get_UsualAllowance, UserLogin, ref status);

            if (listUsualAllowance != null)
            {
                foreach (var i in listUsualAllowance)
                {
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UsualAllowanceName, ElementCode = i.Code, Formula = i.Formula });
                }
            }
            #endregion

            #region Add thêm các phần tử là phụ cấp bất thường
            List<Cat_UnusualAllowanceCfgEntity> listUnusualAllowanceCfg = new List<Cat_UnusualAllowanceCfgEntity>();
            listModel = new List<object>();
            listModel.AddRange(new object[6]);
            listModel[4] = 1;
            listModel[5] = Int32.MaxValue - 1;
            listUnusualAllowanceCfg = baseService.GetData<Cat_UnusualAllowanceCfgEntity>(listModel, ConstantSql.hrm_cat_sp_get_UnusualAllowanceCfg, UserLogin, ref status);
            if (listUnusualAllowanceCfg != null)
            {
                foreach (var i in listUnusualAllowanceCfg)
                {
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code, Formula = i.Formula });

                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_N_6", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_N_5", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_N_4", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_N_3", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_N_2", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_N_1", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_AMOUNTOFOFFSET_N_1", Formula = i.Formula });

                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_T3", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_TIMELINE", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_TIMELINE_N_1", Formula = i.Formula });

                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_DAYCLOSE_N_1", Formula = i.Formula });
                    listCat_Element.Add(new CatElementModel() { ElementName = i.UnusualAllowanceCfgName, ElementCode = i.Code + "_DAYCLOSE", Formula = i.Formula });

                }
            }
            #endregion

            #region Các Phần tử loại công việc [04/07/2016][HienNguyen][68821]
            List<Cat_JobTypeEntity> listJobType = new List<Cat_JobTypeEntity>();
            listModel = Common.AddRange(4);
            listJobType = baseService.GetData<Cat_JobTypeEntity>(listModel, ConstantSql.hrm_cat_sp_get_JobType, UserLogin, ref status);

            if (listJobType != null)
            {
                foreach (var i in listJobType)
                {
                    listCat_Element.Add(new CatElementModel() { ElementName = i.JobTypeName, ElementCode = "ATT_PROFILE_TIMESHEET_" + i.Code + "_NOT_NORMAL_DAY", Formula = "[ATT_PROFILE_TIMESHEET_" + i.Code + "_NOT_NORMAL_DAY]" });
                }
            }
            #endregion

            //Trường hợp mã phần tử trùng với các phần tử hệ thống
            if (listCat_Element.Where(m => m.GradePayrollID == null && m.IsApplyGradeAll == null).Any(m => m.ElementCode == elementCode))
            {
                return Json(new { success = false, mess = ConstantDisplay.HRM_Sal_ElementCode_Dupicate.TranslateString() });
            }

            #region Kiểm tra phần tử (kiểu mới)
            string formula = values.Replace("/t", "").Replace("/n", "").Trim();
            foreach (var i in listCat_Element)
            {
                string _tmp = "[" + i.ElementCode + "]";
                if (formula.IndexOf(_tmp) != -1)
                {
                    //xử lý trường hợp đặt enum phần tử trong dấu ""
                    if (formula.Contains("]\""))
                    {
                        formula = formula.Replace("\"[", "[").Replace("]\"", "]");
                    }
                    //formula = formula.Replace(_tmp, " 1 ");
                    formula = formula.Replace(_tmp, " \"1\" ");
                }
            }

            //kiểm tra trường hợp có sử dụng phần tử động
            //[22122015][bang.nguyen][62095]
            //số ngày có giờ OT nghỉ bù chương trình báo công thức sai
            if (formula.IndexOf(PayrollElement.DYN_COUNTDAYOVERTIMEBYTYPE_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNTLUNCHDAYOVERTIME_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNT_DAY_OVERTIME_BIG_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNT_DAY_OVERTIME_SMALLER_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNT_HOURS_OVERTIMEOFF_BIG_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNT_HOURS_OVERTIMEOFF_SMALLER_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNT_SHIFT_WORKPAIDHOURS_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_HRE_COUNT_DEPENDANT_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_OVERTIME_INPUTHOUR_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_COUNT_NIGHT_SHIFT_WORKPAIDHOURS_.ToString()) != -1
                  || formula.IndexOf("DYN_ATT_OVERTIME_") != -1
                  || formula.IndexOf("DYN_ALLOWANCES_") != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_WORKING_DAY_FOR_ATTENDANCE_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_TOTAL_PAID_LEAVEDAY_DAY_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_SAL_BASIC_SALARY_.ToString()) != -1
                  || formula.IndexOf("DYN_SAL_PRODUCTIVE_SUM_") != -1
                  || formula.IndexOf("DYN_SAL_PRODUCTIVETRACKER_") != -1
                  || formula.IndexOf("DYN_SAL_PRODUCTIVE_COUNT_") != -1
                  || formula.IndexOf(PayrollElement.DYN_HRE_COUNT_RELATIVE_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_MIDDLE_SHIFT_.ToString()) != -1
                  || formula.IndexOf("DYN_PROFILETIMESHEET_") != -1
                  || formula.IndexOf("ATT_ATTTABLEITEM_SUM_LEAVEDAYS_") != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_PREGNANCY_FOR_ATTENDANCE_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_ATT_SUM_SHIFT_PAIDHOURS_NOT_LATEEARLY_.ToString()) != -1
                  || formula.IndexOf(PayrollElement.DYN_SALARY_DEPARTMENT_ELEMENT_.ToString()) != -1
                  || (formula.IndexOf("DYN_ATT_OVERTIME_") != -1 && formula.IndexOf("_SULATEEARLY_TOTALHOURS") != -1)
                  || (formula.IndexOf(PayrollElement.DYN_ATT_OVERTIME_COUNT_BY_SHIFTCODE_.ToString()) != -1)
                  || (formula.IndexOf(PayrollElement.DYN1_ATT_OVERTIME_COUNT_BY_SHIFTCODE_.ToString()) != -1)
                )
            {
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }

            if (formula.IndexOf(PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_WORKPAIDHOURS_.ToString()) != -1)
            {
                string StartsWith = PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_WORKPAIDHOURS_.ToString();
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(StartsWith)).ToList();
                foreach (var objformular in lstFormularCheck)
                {
                    var lstTemp = objformular.Replace("[", "").Replace("]", "").Split('_').ToList();
                    var _strvalue = lstTemp[lstTemp.Count() - 1].ToString();
                    double _valueOutPut = 0;
                    if (!double.TryParse(_strvalue, out _valueOutPut))
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }

            //nếu dạng này thì kiểm tra những phần tử đúng mới dc lưu
            if (formula.IndexOf(PayrollElement.DYN_IS_CONTINOUS_NIGHTSHIFT_.ToString()) != -1)
            {
                string StartsWith = PayrollElement.DYN_IS_CONTINOUS_NIGHTSHIFT_.ToString();
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(StartsWith)).ToList();
                foreach (var objformular in lstFormularCheck)
                {
                    var lstTemp = objformular.Replace("[", "").Replace("]", "").Split('_').ToList();
                    var _strvalue = lstTemp[lstTemp.Count() - 1].ToString();
                    double _valueOutPut = 0;
                    if (!double.TryParse(_strvalue, out _valueOutPut))
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }
            //công thức chứa phần tử 
            if (formula.IndexOf(PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_LEAVEDAY_.ToString()) != -1)
            {
                string StartsWith = PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_LEAVEDAY_.ToString();
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(StartsWith)).ToList();

                List<object> lstpara = new List<object>();
                lstpara.AddRange(new object[4]);
                lstpara[2] = 1;
                lstpara[3] = Int32.MaxValue - 1;
                var lstLeaveDayType = baseService.GetData<Cat_LeaveDayTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_LeaveDayType, UserLogin, ref status);
                foreach (var objformular in lstFormularCheck)
                {
                    //DYN_ATT_ATTENDANCETABLEITEM_COUNT_LEAVEDAY_PGP_1
                    var _element = objformular.Replace("[", "").Replace("]", "");
                    var lstTemp = _element.Split('_').ToList();
                    var _strvalue = lstTemp[lstTemp.Count() - 1].ToString();
                    double _valueOutPut = 0;
                    if (!double.TryParse(_strvalue, out _valueOutPut))
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                    //lấy mã loại nghỉ được cấu hình trong phần tử
                    int indexVualue = _element.IndexOf(_strvalue);
                    int lenghtDYN = PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_LEAVEDAY_.ToString().Length;
                    string _codeLeaveDayTypeFormular = _element.Substring(lenghtDYN, indexVualue - lenghtDYN - 1);
                    var lstLeaveDayByCode = lstLeaveDayType.Where(s => s.Code == _codeLeaveDayTypeFormular).ToList();
                    if (lstLeaveDayByCode.Count == 0)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }
            //[28/04/2016][bang.nguyen][81338][Modify Func]
            if (formula.IndexOf(PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_OVERTIME_.ToString()) != -1)
            {
                string StartsWith = PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_OVERTIME_.ToString();
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(StartsWith)).ToList();

                List<object> lstpara = new List<object>();
                lstpara.AddRange(new object[5]);
                lstpara[3] = 1;
                lstpara[4] = Int32.MaxValue - 1;
                var lstOverTimeType = baseService.GetData<Cat_OvertimeTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_OvertimeType, UserLogin, ref status);
                foreach (var objformular in lstFormularCheck)
                {
                    //DYN_ATT_ATTENDANCETABLEITEM_COUNT_OVERTIME_
                    var _element = objformular.Replace("[", "").Replace("]", "");
                    var lstTemp = _element.Split('_').ToList();
                    var _strvalue = lstTemp[lstTemp.Count() - 1].ToString();
                    double _valueOutPut = 0;
                    if (!double.TryParse(_strvalue, out _valueOutPut))
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                    //lấy mã loại nghỉ được cấu hình trong phần tử
                    int indexVualue = _element.IndexOf(_strvalue);
                    int lenghtDYN = PayrollElement.DYN_ATT_ATTENDANCETABLEITEM_COUNT_OVERTIME_.ToString().Length;
                    string _codeOverTimeTypeFormular = _element.Substring(lenghtDYN, indexVualue - lenghtDYN - 1);
                    var lstOverTimeByCode = lstOverTimeType.Where(s => s.Code == _codeOverTimeTypeFormular).ToList();
                    if (lstOverTimeByCode.Count == 0)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }

            if (formula.IndexOf("ATT_LEAVEDAYTYPE_SUM_") != -1 && formula.IndexOf("_BEFORE_CHANGE_CONTRACT") != -1)
            {
                string StartsWith = "ATT_LEAVEDAYTYPE_SUM_";
                string EndWith = "_BEFORE_CHANGE_CONTRACT";
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(StartsWith) && m.Replace("[", "").Replace("]", "").EndsWith(EndWith)).ToList();

                List<object> lstpara = new List<object>();
                lstpara.AddRange(new object[4]);
                lstpara[2] = 1;
                lstpara[3] = Int32.MaxValue - 1;
                var lstLeaveDayType = baseService.GetData<Cat_LeaveDayTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_LeaveDayType, UserLogin, ref status);
                lstLeaveDayType = lstLeaveDayType.Where(s => s.Code != null && s.Code != string.Empty).ToList();
                foreach (var objformular in lstFormularCheck)
                {
                    //lấy mã loại nghỉ được cấu hình trong phần tử
                    string _codeLeaveDayTypeFormular = objformular.Replace("[", "").Replace("]", "").Replace(StartsWith, "").Replace(EndWith, "");
                    var lstLeaveDayByCode = lstLeaveDayType.Where(s => s.Code == _codeLeaveDayTypeFormular).ToList();
                    if (lstLeaveDayByCode.Count == 0)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }

            if (formula.IndexOf("ATT_LEAVEDAYTYPE_SUM_") != -1 && formula.IndexOf("_AFTER_CHANGE_CONTRACT") != -1)
            {
                string StartsWith = "ATT_LEAVEDAYTYPE_SUM_";
                string EndWith = "_AFTER_CHANGE_CONTRACT";
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(StartsWith) && m.Replace("[", "").Replace("]", "").EndsWith(EndWith)).ToList();

                List<object> lstpara = new List<object>();
                lstpara.AddRange(new object[4]);
                lstpara[2] = 1;
                lstpara[3] = Int32.MaxValue - 1;
                var lstLeaveDayType = baseService.GetData<Cat_LeaveDayTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_LeaveDayType, UserLogin, ref status);
                lstLeaveDayType = lstLeaveDayType.Where(s => s.Code != null && s.Code != string.Empty).ToList();
                foreach (var objformular in lstFormularCheck)
                {
                    //lấy mã loại nghỉ được cấu hình trong phần tử
                    string _codeLeaveDayTypeFormular = objformular.Replace("[", "").Replace("]", "").Replace(StartsWith, "").Replace(EndWith, "");
                    var lstLeaveDayByCode = lstLeaveDayType.Where(s => s.Code == _codeLeaveDayTypeFormular).ToList();
                    if (lstLeaveDayByCode.Count == 0)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }

            //[21/08/2017][bang.nguyen][86779][Modify Func]
            if (formula.IndexOf("DYN_SALREUSE_") != -1 && formula.IndexOf("_N_") != -1)
            {
                string _startWith = "DYN_SALREUSE_";
                var listEndWithMonthRe = new string[] { "_N_1", "_N_2", "_N_3", "_N_4", "_N_5", "_N_6", "_N_7", "_N_8", "_N_9", "_N_10", "_N_11", "_N_12" };
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();

                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(_startWith)
                && listEndWithMonthRe.Any(s => m.Contains(s))).ToList();
                if (lstFormularCheck.Count > 0)
                {
                    foreach (var objformular in lstFormularCheck)
                    {
                        var _element = objformular.Replace("[", "").Replace("]", "");
                        _element = _element.Replace(_startWith, "");
                        foreach (var endWithmonthRe in listEndWithMonthRe)
                        {
                            if (_element.EndsWith(endWithmonthRe))
                            {
                                var _value = endWithmonthRe.Replace("_N_", "");
                                int _valueOutPut = 1;
                                //giá trị nhập vào đúng là số
                                if (!int.TryParse(_value, out _valueOutPut))
                                {
                                    return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                                }
                            }
                        }
                    }
                    return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
                }
                else
                {
                    return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                }
            }

            //[16/10/2017][bang.nguyen][88650][modify]
            if (formula.IndexOf("DYN_ATT_COUNTLUNCHDAY_") != -1)
            {
                string _startWith = "DYN_ATT_COUNTLUNCHDAY_";
                var listEndWith = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();

                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(_startWith)
                && listEndWith.Any(s => m.Contains(s))).ToList();
                if (lstFormularCheck.Count > 0)
                {
                    lstFormularCheck = lstFormularCheck.Select(s => s.Replace("[", "").Replace("]", "")).ToList();
                    List<object> lstpara = new List<object>();
                    lstpara.AddRange(new object[5]);
                    lstpara[3] = 1;
                    lstpara[4] = Int32.MaxValue - 1;
                    var lstOverTimeType = baseService.GetData<Cat_OvertimeTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_OvertimeType, UserLogin, ref status);
                    foreach (var _element in lstFormularCheck)
                    {
                        string paramNumber = _element.Split('_').LastOrDefault();
                        int NumberHours = 0;

                        if (!int.TryParse(paramNumber, out NumberHours))
                        {
                            return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                        }
                        var codeOT = _element.Replace(_startWith, "").Replace("_" + paramNumber, "");
                        var objOverTimeType = lstOverTimeType.Where(s => s.Code == codeOT).FirstOrDefault();
                        if (objOverTimeType == null)
                        {
                            return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                        }
                    }
                    return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
                }
                else
                {
                    return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                }
            }

            //[17/10/2017][bang.nguyen][88781][New Func]
            if (formula.IndexOf("DYN_ATT_LEAVEDAY_VIOLATE_MIN_INFORM_DAY_") != -1)
            {
                string _startWith = "DYN_ATT_LEAVEDAY_VIOLATE_MIN_INFORM_DAY_";
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();

                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(_startWith)).ToList();
                if (lstFormularCheck.Count > 0)
                {
                    lstFormularCheck = lstFormularCheck.Select(s => s.Replace("[", "").Replace("]", "")).ToList();
                    List<object> lstpara = new List<object>();
                    lstpara.AddRange(new object[4]);
                    lstpara[2] = 1;
                    lstpara[3] = Int32.MaxValue - 1;
                    var lstLeaveDayType = baseService.GetData<Cat_LeaveDayTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_LeaveDayType, UserLogin, ref status);

                    foreach (var _element in lstFormularCheck)
                    {
                        string paramNumber = _element.Split('_').LastOrDefault();
                        int NumberDays = 0;

                        if (!int.TryParse(paramNumber, out NumberDays))
                        {
                            return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                        }
                        var codeLeaveDayType = _element.Replace(_startWith, "").Replace("_" + paramNumber, "");
                        var objLeaveDayType = lstLeaveDayType.Where(s => s.Code == codeLeaveDayType).FirstOrDefault();
                        if (objLeaveDayType == null)
                        {
                            return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                        }
                    }
                    return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
                }
                else
                {
                    return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                }
            }

            //[17/10/2017][bang.nguyen][88781][New Func]
            if (formula.IndexOf("DYN_ATT_LEAVEDAY_COUNT_LEAVEDAYS_BEFORE_QUIT_") != -1)
            {
                string _startWith = "DYN_ATT_LEAVEDAY_COUNT_LEAVEDAYS_BEFORE_QUIT_";
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();

                List<string> lstformula = Common.ParseFormulaToList(_formula);
                var lstFormularCheck = lstformula.Where(m => m.Replace("[", "").Replace("]", "").StartsWith(_startWith)).ToList();
                if (lstFormularCheck.Count > 0)
                {
                    lstFormularCheck = lstFormularCheck.Select(s => s.Replace("[", "").Replace("]", "")).Distinct().ToList();
                    List<object> lstpara = new List<object>();
                    lstpara.AddRange(new object[4]);
                    lstpara[2] = 1;
                    lstpara[3] = Int32.MaxValue - 1;
                    var lstLeaveDayType = baseService.GetData<Cat_LeaveDayTypeEntity>(lstpara, ConstantSql.hrm_cat_sp_get_LeaveDayType, UserLogin, ref status);

                    foreach (var _element in lstFormularCheck)
                    {
                        var codeLeaveDayType = _element.Replace(_startWith, "");
                        var objLeaveDayType = lstLeaveDayType.Where(s => s.Code == codeLeaveDayType).FirstOrDefault();
                        if (objLeaveDayType == null)
                        {
                            return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                        }
                    }
                    return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
                }
                else
                {
                    return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                }
            }
            //[16/01/2018][bang.nguyen][92094][New Func]
            if (formula.IndexOf(PayrollElement.DYN_ATT_ATTENDANCETABLE_COUNT_ACTUALWORKHOUR_.ToString()) != -1 && formula.IndexOf("_BYFROMANDTO") != -1)
            {
                var strStartsWith = PayrollElement.DYN_ATT_ATTENDANCETABLE_COUNT_ACTUALWORKHOUR_.ToString();
                var strEndWith = "_BYFROMANDTO";
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                lstformula = lstformula.Select(s => s.Replace("[", "").Replace("]", "")).ToList();
                List<string> lstFormularCheck = lstformula.Where(m => m.StartsWith(strStartsWith) && m.EndsWith(strEndWith)).Distinct().ToList();

                foreach (var formulaitem in lstFormularCheck)
                {
                    var strFormula = formulaitem;
                    var strHourFromAndTo = strFormula.Replace(strStartsWith, "").Replace(strEndWith, "");
                    if (!string.IsNullOrEmpty(strHourFromAndTo))
                    {
                        var listHourFromAndTo = strHourFromAndTo.Split("_").ToList();
                        if (listHourFromAndTo.Count == 2)
                        {
                            double valueHourFrom = 0;
                            double valueHourTo = 0;

                            if (!double.TryParse(listHourFromAndTo[0], out valueHourFrom) || !double.TryParse(listHourFromAndTo[1], out valueHourTo))
                            {
                                return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }

            #region xử lý check phần tử nhóm động
            //[17/01/2018][bang.nguyen][0092408][Modify Func]
            //ELEMENT_GROUP_DYN_SAL_PERFORMANCEALLOWANCE_AMOUNT_+ mã UnusualAllowanceCfg + _BYCODEUNUSUALALLOWANCECFG
            if (formula.IndexOf(PayrollElementGroup.ELEMENT_GROUP_DYN_SAL_PERFORMANCEALLOWANCE_AMOUNT_.ToString()) != -1 && formula.IndexOf("_BYCODEUNUSUALALLOWANCECFG") != -1)
            {
                var strStartsWith = PayrollElementGroup.ELEMENT_GROUP_DYN_SAL_PERFORMANCEALLOWANCE_AMOUNT_.ToString();
                var strEndWith = "_BYCODEUNUSUALALLOWANCECFG";
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                lstformula = lstformula.Select(s => s.Replace("[", "").Replace("]", "")).ToList();
                List<string> lstFormularCheck = lstformula.Where(m => m.StartsWith(strStartsWith) && m.EndsWith(strEndWith)).Distinct().ToList();

                foreach (var formulaitem in lstFormularCheck)
                {
                    var strFormula = formulaitem;
                    var codeUnusualAllowanceCfg = strFormula.Replace(strStartsWith, "").Replace(strEndWith, "");

                    var objUnusualAllowanceCfg = listUnusualAllowanceCfg.Where(s => s.Code == codeUnusualAllowanceCfg).FirstOrDefault();
                    if (objUnusualAllowanceCfg == null)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }
            #endregion
            #region check phan tu goi du lieu cua phan tu nhom
            //Cho phép phần tử lương gọi phần tử lương nhóm
            //[17/01/2018][bang.nguyen][92410][Modify Func]
            if (formula.IndexOf("GROUP_DYN_SUM_") != -1 && formula.IndexOf("_BYCODEELEMENTGROUP") != -1)
            {
                var strStartsWith = "GROUP_DYN_SUM_";
                var strEndWith = "_BYCODEELEMENTGROUP";
                //loại bỏ các khoản trắng và xuống dòng
                var _formula = formula.Replace("\n", "").Replace("\t", "").Trim();
                //Tách các phần tử từ công thức ra một array
                List<string> lstformula = Common.ParseFormulaToList(_formula);
                lstformula = lstformula.Select(s => s.Replace("[", "").Replace("]", "")).ToList();
                List<string> lstFormularCheck = lstformula.Where(m => m.StartsWith(strStartsWith) && m.EndsWith(strEndWith)).Distinct().ToList();
                var listElementIsGroup = listCat_Element.Where(s => s.IsPayrollGroupElement == true).ToList();

                foreach (var formulaitem in lstFormularCheck)
                {
                    var strFormula = formulaitem;
                    var codeElementGroup = strFormula.Replace(strStartsWith, "").Replace(strEndWith, "");

                    var objElementIsGroup = listElementIsGroup.Where(s => s.ElementCode == codeElementGroup).FirstOrDefault();
                    if (objElementIsGroup == null)
                    {
                        return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                    }
                }
                return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
            }
            #endregion

            try
            {
                FormulaHelper.FormulaHelperModel value = FormulaHelper.ParseFormula(formula, new List<ElementFormula>());
                if (value.ErrorMessage == string.Empty)
                {
                    return Json(new { success = true, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaSuccess.Translate() });
                }
                else
                {
                    return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
                }
            }
            catch
            {
                return Json(new { success = false, mess = ConstantDisplay.HRM_Category_Element_CheckFormulaFail.Translate() });
            }
            #endregion
        }

        /// <summary>
        ///  [Hien.Nguyen] Kiểm tra xem công thức có hợp lệ hay không
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckFormulaUsualAllowance(string values)
        {
            if (values.Replace("\n", "").Replace(" ", "") == string.Empty)
            {
                return Json(new { success = false, data = 0 });
            }
            values = values.Replace("\n", "");
            string status = string.Empty;
            List<CatElementModel> listCat_Element = new List<CatElementModel>();

            #region Add thêm các phần tử Enum để kiểm tra
            var valuesAsList = Enum.GetValues(typeof(PayrollElement)).Cast<PayrollElement>().ToList();
            foreach (var i in valuesAsList)
            {
                listCat_Element.Add(new CatElementModel() { ElementCode = i.ToString(), Formula = "[" + i.ToString() + "]" });
            }
            #endregion

            #region Kiểm tra phần tử và các phép toán
            string formula = values.Replace(" ", "").Replace("+", " ").Replace("-", " ").Replace("*", " ").Replace("/", " ");
            if (formula.Contains("  "))//trường hợp 2 phương thức đứng chung (++,--)
            {
                string errorValue = values[formula.IndexOf("  ")].ToString();
                return Json(new { success = false, data = errorValue });
            }
            List<string> listFormula = formula.Split(' ').ToList();

            #region Kiểm tra dấu đóng mở phải là từng cặp
            int Open = values.Count(m => m == '(');
            int Close = values.Count(m => m == ')');
            if (Open != Close)
            {
                return Json(new { success = false, data = "(" });
            }
            #endregion
            for (int i = 0; i < listFormula.Count; i++)
            {
                if (listFormula[i].Contains("("))
                {
                    if (!listFormula[i].StartsWith("("))
                    {
                        return Json(new { success = false, data = listFormula[i] });
                    }
                }
                if (listFormula[i].Contains(")"))
                {
                    if (!listFormula[i].EndsWith(")"))
                    {
                        return Json(new { success = false, data = listFormula[i] });
                    }
                }

                listFormula[i] = listFormula[i].Replace("(", "").Replace(")", "");

                if (listFormula[i] == "")//Loại bỏ trường hợp phần tử rỗng
                {
                    return Json(new { success = false, data = i });
                }
                if (listFormula[i].StartsWith("[") && listFormula[i].EndsWith("]"))//Là phần tử tính lương
                {
                    if (!listCat_Element.Any(m => m.ElementCode == listFormula[i].Replace("[", "").Replace("]", "")))
                    {
                        return Json(new { success = false, data = listFormula[i] });
                    }
                }
                else//Là số
                {
                    if (!Common.IsNumber(listFormula[i]))
                    {
                        return Json(new { success = false, data = listFormula[i] });
                    }
                }
            }
            #endregion
            return Json(new { success = true });
        }

        #endregion

        #region Cat_RateInsurance

        [HttpPost]
        public ActionResult GetRateInsuranceList([DataSourceRequest] DataSourceRequest request, Cat_RateInsuranceSearchModel model)
        {
            return GetListDataAndReturn<Cat_RateInsuranceModel, Cat_RateInsuranceEntity, Cat_RateInsuranceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RateInsurance);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Tỷ Lệ Bảo Hiểm (Cat_RateInsurance) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllRateInsuranceList([DataSourceRequest] DataSourceRequest request, Cat_RateInsuranceSearchModel model)
        {
            //Tung.Tran 20172510 Code xuất riêng để hiển thị đúng tỉ lệ %
            model.SetPropertyValue("IsExport", true);
            string fullPath = string.Empty, status = string.Empty;
            var listModel = GetListData<Cat_RateInsuranceModel, Cat_RateInsuranceEntity, Cat_RateInsuranceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RateInsurance, ref status);
            if (status == NotificationType.Success.ToString())
            {
                if (listModel.Any())
                {
                    foreach (var item in listModel)
                    {
                        item.SocialInsCompRate = item.SocialInsCompRate * 100;
                        item.SocialInsEmpRate = item.SocialInsEmpRate * 100;
                        item.UnemployInsCompRate = item.UnemployInsCompRate * 100;
                        item.UnemployInsEmpRate = item.UnemployInsEmpRate * 100;
                        item.HealthInsCompRate = item.HealthInsCompRate * 100;
                        item.HealthInsEmpRate = item.HealthInsEmpRate * 100;
                    }
                }
                status = ExportService.ExportAll(listModel, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));
            }
            return Json(status);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn củaTỷ Lệ Bảo Hiểm (Cat_RateInsurance) theo điều kiện tìm kiếm
        public ActionResult ExportRateInsuranceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RateInsuranceEntity, Cat_RateInsuranceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RateInsuranceByIds);
        }
        #endregion

        #region Cat_ValueEntity

        [HttpPost]
        public ActionResult GetValueEntityList([DataSourceRequest] DataSourceRequest request, Cat_ValueEntitySearchModel model)
        {
            return GetListDataAndReturn<Cat_ValueEntityModel, Cat_ValueEntityEntity, Cat_ValueEntitySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ValueEntity);
        }

        /// [Quoc.Do] - Xuất danh sách dữ liệu cho Tỷ Lệ Bảo Hiểm (Cat_RateInsurance) theo điều kiện tìm kiếm
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllValueEntityList([DataSourceRequest] DataSourceRequest request, Cat_ValueEntitySearchModel model)
        {
            return ExportAllAndReturn<Cat_ValueEntityEntity, Cat_ValueEntityModel, Cat_ValueEntitySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ValueEntity);
        }

        /// [Quoc.Do] - Xuất các dòng dữ liệu được chọn củaTỷ Lệ Bảo Hiểm (Cat_RateInsurance) theo điều kiện tìm kiếm
        public ActionResult ExportValueEntitySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ValueEntityEntity, Cat_ValueEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ValueEntityByIds);
        }
        #endregion

        #region Cat_Shop
        public ActionResult GetShopDataByID(Guid? shopID)
        {
            if (shopID != null)
            {
                var actionserveice = new ActionService(UserLogin);
                string status = string.Empty;
                var _ProvinceEntity = actionserveice.GetByIdUseStore<Cat_ShopEntity>(shopID.Value, ConstantSql.hrm_cat_sp_get_ShopById, ref status);
                if (_ProvinceEntity != null)
                    return Json(_ProvinceEntity, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult CloseShop(string selectedIds, DateTime? dateClose)
        {
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var listIDs = selectedIds.Split(',').Select(x => Guid.Parse(x)).ToList();
                var profileServices = new Hre_ProfileServices();
                string messageRef = profileServices.CloseShop(listIDs, dateClose);
                return Json(messageRef);
            }
            return Json(null);
        }

        [HttpPost]
        //Son.Vo - 20161117 - 0075764
        public ActionResult CheckDateCloseShop(string selectedIds)
        {
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var profileServices = new Hre_ProfileServices();
                var listIDs = selectedIds.Split(',').Select(x => Guid.Parse(x)).ToList();
                bool isClose = profileServices.CheckShopClosed(listIDs);
                return Json(isClose, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetShopList([DataSourceRequest] DataSourceRequest request, Cat_ShopSearchModel model)
        {
            return GetListDataAndReturn<Cat_ShopModel, Cat_ShopEntity, Cat_ShopSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Shop);
        }

        [HttpPost]
        public JsonResult GetMultiShop(string text)
        {
            return GetDataForControl<Cat_ShopMultiModel, Cat_ShopMultiEntity>(text, ConstantSql.hrm_Cat_sp_get_Shop_multi);
        }

        [HttpPost]
        public JsonResult GetTreeViewShop(Guid? ID)
        {
            List<object> listModel = new List<object>();
            var service = new BaseService();
            string status = string.Empty;

            if (ID == null || ID == Guid.Empty)//Load Group Shop
            {
                List<Cat_ShopGroupEntity> listGroup = new List<Cat_ShopGroupEntity>();
                listModel = new List<object>();
                listModel.AddRange(new object[4]);
                listModel[2] = 1;
                listModel[3] = Int32.MaxValue - 1;
                listGroup = service.GetData<Cat_ShopGroupEntity>(listModel, ConstantSql.hrm_cat_sp_get_ShopGroup, UserLogin, ref status);
                var result = from e in listGroup
                             select new
                             {
                                 id = e.ID,
                                 Name = e.ShopGroupName,
                                 hasChildren = e.CountShop > 0 ? true : false,
                                 Group = 0,
                                 IsShow = true,
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<Cat_ShopEntity> listShop = new List<Cat_ShopEntity>();
                listModel = new List<object>();
                listModel.AddRange(new object[5]);
                listModel[0] = ID;
                listModel[3] = 1;
                listModel[4] = Int32.MaxValue - 1;
                listShop = service.GetData<Cat_ShopEntity>(listModel, ConstantSql.hrm_cat_sp_get_Shop, UserLogin, ref status);
                var result = from e in listShop
                             select new
                             {
                                 id = e.ID,
                                 Name = e.ShopName,
                                 hasChildren = false,
                                 Group = 1,
                                 IsShow = true,

                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportAllCat_ShopList([DataSourceRequest] DataSourceRequest request, Cat_ShopSearchModel model)
        {
            return ExportAllAndReturn<Cat_ShopModel, Cat_ShopEntity, Cat_ShopSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Shop);
        }

        public ActionResult ExportCat_ShopSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ShopEntity, Cat_ShopModel>(selectedIds, valueFields, ConstantSql.hrm_Cat_sp_get_ShopIds);
        }

        public JsonResult GetShopByOrgID(Guid orgID, string ShopFilter)
        {
            var result = new List<Cat_ShopModel>();
            string status = string.Empty;
            if (orgID != Guid.Empty)
            {
                var service = new Cat_ShopServices();
                result = service.GetData<Cat_ShopModel>(orgID, ConstantSql.hrm_Cat_sp_get_ShopbyOrgID, UserLogin, ref status);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHDTJobTypeByHDTJobGroupID(Guid HDTJobGroup, string HDTJobTypeFilter)
        {
            var result = new List<Cat_HDTJobTypeModel>();
            string status = string.Empty;
            if (HDTJobGroup != Guid.Empty)
            {
                var service = new Cat_HDTJobTypeServices();
                result = service.GetData<Cat_HDTJobTypeModel>(HDTJobGroup, ConstantSql.hrm_Cat_sp_get_HDTJobTypeByGroupID, UserLogin, ref status);
                if (!string.IsNullOrEmpty(HDTJobTypeFilter))
                {
                    var rs = result.Where(s => s.HDTJobTypeName != null && s.HDTJobTypeName.ToLower().Contains(HDTJobTypeFilter.ToLower())).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHDTJobTypeCodeByHDTJobGroupID(Guid HDTJobGroupID, string filterHDTJob)
        {
            var result = new List<Cat_HDTJobTypeCodeMultiModel>();
            string status = string.Empty;
            List<object> obj = new List<object>();
            obj.Add(HDTJobGroupID);
            obj.Add(filterHDTJob);
            //if (HDTJobGroupID != Guid.Empty)
            //{
            var service = new Cat_HDTJobTypeServices();
            result = service.GetData<Cat_HDTJobTypeCodeMultiModel>(obj, ConstantSql.hrm_Cat_sp_get_HDTJobTypeCodeByGroupID, UserLogin, ref status);
            //}
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTypeHDTJobGroupID(Guid ID)
        {
            string status = string.Empty;
            if (ID != Guid.Empty)
            {
                var service = new Cat_HDTJobGroupServices();
                var data = service.GetData<Cat_HDTJobGroupModel>(Common.DotNetToOracle(ID.ToString()), ConstantSql.hrm_cat_sp_get_HDTJobGroupById, UserLogin, ref status);
                if (data == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                var result = from e in data
                             select new
                             {
                                 Text = e.Type.TranslateString(),
                                 Value = e.Type,
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTypeHDTJobTypeID(Guid ID)
        {
            string status = string.Empty;
            if (ID != Guid.Empty)
            {
                var service = new Cat_HDTJobTypeServices();
                var data = service.GetData<Cat_HDTJobTypeModel>(Common.DotNetToOracle(ID.ToString()), ConstantSql.hrm_cat_sp_get_HDTJobTypeById, UserLogin, ref status);
                if (data == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                var result = from e in data
                             select new
                             {
                                 GroupName = e.HDTJobGroupName,
                                 GroupID = e.HDTJobGroupID,
                                 Text = e.Type.TranslateString(),
                                 Value = e.Type,
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApproveHDTJobTypePrice(List<Guid> selectedIds)
        {
            var service = new Cat_HDTJobTypePriceServices();
            var message = service.ActionApproved(selectedIds);
            return Json(message);
        }


        #endregion

        #region Cat_Role
        [HttpPost]
        public ActionResult GetRoleList([DataSourceRequest] DataSourceRequest request, Cat_RoleSearchModel model)
        {
            return GetListDataAndReturn<Cat_RoleModel, Cat_RoleEntity, Cat_RoleSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Role);
        }

        public ActionResult ExportRoleSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RoleEntity, Cat_RoleModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RoleByIds);
        }

        public JsonResult GetMultiRole(string text)
        {
            return GetDataForControl<Cat_RoleMultiModel, Cat_RoleMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Role_multi);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllRoleList([DataSourceRequest] DataSourceRequest request, Cat_RoleSearchModel model)
        {
            return ExportAllAndReturn<Cat_RoleEntity, Cat_RoleModel, Cat_RoleSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Role);
        }
        #endregion

        #region Cat_JobType
        [HttpPost]
        public ActionResult GetJobTypeList([DataSourceRequest] DataSourceRequest request, Cat_JobTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_JobTypeModel, Cat_JobTypeEntity, Cat_JobTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_JobType);
        }
        //public ActionResult ExportAllReasonDenylList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        //{
        //    return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ResonDeny);
        //}

        public ActionResult ExportJobTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_JobTypeEntity, Cat_JobTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_JobTypeByIds);
        }

        public JsonResult GetMultiJobType(string text)
        {
            return GetDataForControl<Cat_JobTypeMultiModel, Cat_JobTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_JobType_multi);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllJobTypeList([DataSourceRequest] DataSourceRequest request, Cat_JobTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_JobTypeEntity, Cat_JobTypeModel, Cat_JobTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_JobType);
        }
        #endregion

        #region Cat_UnitPrice
        [HttpPost]
        public ActionResult GetUnitPriceList([DataSourceRequest] DataSourceRequest request, Cat_UnitPriceSearchModel model)
        {
            return GetListDataAndReturn<Cat_UnitPriceModel, Cat_UnitPriceEntity, Cat_UnitPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UnitPrice);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllUnitPriceList([DataSourceRequest] DataSourceRequest request, Cat_UnitPriceSearchModel model)
        {
            return ExportAllAndReturn<Cat_UnitPriceEntity, Cat_UnitPriceEntity, Cat_UnitPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UnitPrice);
        }

        //public ActionResult ExportAllReasonDenylList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        //{
        //    return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ResonDeny);
        //}

        public ActionResult ExportUnitPriceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_UnitPriceEntity, Cat_UnitPriceEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_UnitPriceByIds);
        }

        public JsonResult GetJobTypeNameByRoleID(Guid roleid)
        {
            var result = new List<Cat_JobTypeMultiModel>();
            string status = string.Empty;
            if (roleid != Guid.Empty)
            {
                var service = new Cat_UnitPriceServices();
                result = service.GetData<Cat_JobTypeMultiModel>(roleid, ConstantSql.hrm_cat_sp_get_JobTypeByRoleId, UserLogin, ref status);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cat_HDTJobTypePrice

        [HttpPost]
        public ActionResult GetHDTJobTypePriceList([DataSourceRequest] DataSourceRequest request, Cat_HDTJobTypePriceSearchModel model)
        {
            return GetListDataAndReturn<Cat_HDTJobTypePriceModel, Cat_HDTJobTypePriceEntity, Cat_HDTJobTypePriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_HDTJobTypePrice);
        }

        public ActionResult ExportAllHDTJobTypePriceList([DataSourceRequest] DataSourceRequest request, Cat_HDTJobTypePriceSearchModel model)
        {
            return ExportAllAndReturn<Cat_HDTJobTypePriceEntity, Cat_HDTJobTypePriceModel, Cat_HDTJobTypePriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_HDTJobTypePrice);
        }

        public ActionResult ExportHDTJobTypePriceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_HDTJobTypePriceEntity, Cat_HDTJobTypePriceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_HDTJobTypePriceByIds);
        }
        #endregion

        #region Load chế độ công và chế độ lượng theo Rank( code của Cat_SalaryClass)
        [HttpPost]
        public ActionResult GetGradePayrollAndGradeAttendanceByRank(string Rank)
        {
            var serviceGradePayroll = new Cat_GradePayrollServices();
            var serviceGradeAttendance = new Cat_GradeAttendanceServices();
            Guid GradePayrollID = serviceGradePayroll.GetGradePayrollByRank(Rank);
            Guid GradeAttendanceID = serviceGradeAttendance.GetGradeAttendanceByRank(Rank);
            var data = new { GradePayrollID = GradePayrollID, GradeAttendanceID = GradeAttendanceID };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Cat_Topic
        [HttpPost]
        public ActionResult GetTopicList([DataSourceRequest] DataSourceRequest request, Cat_TopicSearchModel model)
        {
            return GetListDataAndReturn<Cat_TopicModel, Cat_TopicEntity, Cat_TopicSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Topic);

        }

        [HttpPost]
        public ActionResult ExportTopicList([DataSourceRequest] DataSourceRequest request, Cat_TopicSearchModel model)
        {
            return ExportAllAndReturn<Cat_TopicEntity, Cat_TopicModel, Cat_TopicSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Topic);
        }

        public JsonResult GetMultiTopic(string text)
        {
            return GetDataForControl<Cat_TopicMultiModel, Cat_TopicMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Topic_Multi);
        }
        #endregion

        public ActionResult ExportBankSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_BankEntity, CatBankModel>(selectedIds, valueFields, ConstantSql.hrm_Cat_sp_get_BankByIds);
        }

        #region cat_MasterDataGroup
        public ActionResult GetCat_MasterDataGroupList([DataSourceRequest] DataSourceRequest request, Cat_MasterDataGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_MasterDataGroupModel, Cat_MasterDataGroupEntity, Cat_MasterDataGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_MasterDataGroup);
        }

        [HttpPost]
        public ActionResult GetMasterGroupItemByMasterGroupIDList([DataSourceRequest] DataSourceRequest request, Guid? masterDataGroupID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(masterDataGroupID);
            var result = baseService.GetData<Cat_MasterDataGroupItemEntity>(objs, ConstantSql.hrm_cat_sp_get_MasterDataGroupItemByMasterDataGroupID, UserLogin, ref status);
            long stt = 1;
            foreach (var item in result)
            {
                item.STT = stt++;
            }
            if (result != null)
            {
                return Json(result.ToDataSourceResult(request));
            }

            return null;
        }
        #endregion

        #region Pur_ProfileNotEligible
        /// <summary>
        /// Lây danh sách nhân viên không đủ điều kiện nhận xe.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetLisProfileNotEligible([DataSourceRequest] DataSourceRequest request, Pur_MCAMSearchModel model)
        {
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            #region Header Info
            List<HeaderInfo> listHeaderInfo = new List<HeaderInfo>();
            HeaderInfo HeaderInfo1 = new HeaderInfo() { Name = "RequestFrom", Value = model.RequestFrom };
            HeaderInfo HeaderInfo2 = new HeaderInfo() { Name = "RequestTo", Value = model.RequestTo };
            HeaderInfo HeaderInfo3 = new HeaderInfo() { Name = "ProfileExport", Value = model.ProfileExport };
            HeaderInfo HeaderInfo4 = new HeaderInfo() { Name = "DateExport", Value = DateTime.Now };
            listHeaderInfo = new List<HeaderInfo>() { HeaderInfo1, HeaderInfo2, HeaderInfo3, HeaderInfo4 };
            #endregion
            #region Load template bao cao
            if (model != null && model.IsCreateTemplate)
            {
                var path = Common.GetPath("Templates");
                ExportService exportService = new ExportService();
                ConfigExport cfgExport = new ConfigExport()
                {
                    Object = new Pur_MCAMModel(),
                    FileName = "LisProfileNotEligible",
                    OutPutPath = path,
                    DownloadPath = "Templates",
                    HeaderInfo = listHeaderInfo,
                    IsDataTable = false

                };
                var str = exportService.CreateTemplate(cfgExport);
                return Json(str);
            }
            #endregion


            #region Load Danh sach nhan vien da nhan xe va loai bo nhung dong trung nhau
            List<object> lstobj = new List<object>();
            int Page = request.Page;
            int PageSize = request.PageSize;
            if (model.ExportID != Guid.Empty)
            {
                Page = 1;
                PageSize = int.MaxValue - 1;
            }
            lstobj.AddRange(new object[24]);
            lstobj[0] = model.OrgStructureID;
            lstobj[1] = model.WorkPlaceID;
            lstobj[2] = model.CodeEmp;
            lstobj[3] = model.ProfileName;
            lstobj[4] = model.ModelType;
            lstobj[5] = model.ModelName;
            lstobj[6] = model.ReceivePlaceID;
            lstobj[7] = model.StartDate;
            lstobj[8] = model.EndDate;
            lstobj[9] = model.ModelCode;
            lstobj[10] = EnumDropDown.StatusCheck.E_CHECKFAIL.ToString();
            lstobj[11] = model.RequestFrom;
            lstobj[12] = model.RequestTo;
            lstobj[13] = model.IsReceive;
            lstobj[14] = model.IsLiquidated;
            lstobj[15] = model.Status;
            lstobj[16] = model.IsPaidDeposit;
            lstobj[17] = model.ReasonNotEligible;
            lstobj[18] = !string.IsNullOrEmpty(model.WorkPlaceIDs) ? Common.DotNetToOracle(model.WorkPlaceIDs) : null;
            lstobj[19] = model.EndDateFrom;
            lstobj[20] = model.EndDateTo;
            lstobj[21] = model.IsLiquitBefor;
            lstobj[22] = Page;
            lstobj[23] = PageSize;
            var result = actionservice.GetData<Pur_MCAMEntity>(lstobj, ConstantSql.Hrm_Pur_SP_GET_PURMCAM, ref status).Translate<Pur_MCAMModel>().ToList();

            var objNameEntity = new List<object>();
            objNameEntity.AddRange(new object[4]);
            objNameEntity[1] = EnumDropDown.EntityType.E_ReasonNotEligible.ToString();
            objNameEntity[2] = 1;
            objNameEntity[3] = Int32.MaxValue - 1;
            var ListNameEntity = actionservice.GetData<Cat_NameEntityEntity>(objNameEntity, ConstantSql.hrm_cat_sp_get_NameEntity, ref status);

            foreach (var item in result)
            {
                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.StatusCheck != null)
                {
                    item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKFAIL.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Fail.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKPASS.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Pass.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = ConstantDisplay.HRM_Category_Pur_MCAM_NotCheck.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                }
                if (item.ReasonNotEligible != null)
                {
                    var _src = ListNameEntity.Where(s => s.Code == item.ReasonNotEligible).FirstOrDefault();
                    item.ReasonNotEligibleTranslate = _src.NameEntityName;
                }
            }
            #endregion

            #region Xuất báo cáo
            if (model.ExportID != Guid.Empty)
            {
                var fullPath = ExportService.Export(model.ExportID, result, listHeaderInfo, UserGuidID, ExportFileType.Excel);
                return Json(fullPath);
            }
            #endregion
            //var _datasource = _data.ToDataSourceResult(request);
            //return Json(_datasource, JsonRequestBehavior.AllowGet);
            var _datasource = result.ToDataSourceResult(request);
            int total = result.FirstOrDefault().GetPropertyValue("TotalRow") != null ? (int)result.FirstOrDefault().GetPropertyValue("TotalRow") : 0;
            _datasource.Total = total;
            _datasource.Data = result;
            return Json(_datasource, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region Pur_ProfileEnoughEligible
        /// <summary>
        /// Lây danh sách nhân viên đủ điều kiện nhận xe.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetLisProfileEnoughEligible([DataSourceRequest] DataSourceRequest request, Pur_MCAMSearchModel model)
        {

            #region Header Info
            List<HeaderInfo> listHeaderInfo = new List<HeaderInfo>();
            HeaderInfo HeaderInfo1 = new HeaderInfo() { Name = "RequestFrom", Value = model.RequestFrom };
            HeaderInfo HeaderInfo2 = new HeaderInfo() { Name = "RequestTo", Value = model.RequestTo };
            HeaderInfo HeaderInfo3 = new HeaderInfo() { Name = "ProfileExport", Value = model.ProfileExport };
            HeaderInfo HeaderInfo4 = new HeaderInfo() { Name = "DateExport", Value = DateTime.Now };
            listHeaderInfo = new List<HeaderInfo>() { HeaderInfo1, HeaderInfo2, HeaderInfo3, HeaderInfo4 };
            #endregion
            #region Load template bao cao
            if (model != null && model.IsCreateTemplate)
            {
                var path = Common.GetPath("Templates");
                ExportService exportService = new ExportService();
                ConfigExport cfgExport = new ConfigExport()
                {
                    Object = new Pur_MCAMModel(),
                    FileName = "ListProfileReceived",
                    OutPutPath = path,
                    DownloadPath = "Templates",
                    HeaderInfo = listHeaderInfo,
                    IsDataTable = false

                };
                var str = exportService.CreateTemplate(cfgExport);
                return Json(str);
            }
            #endregion
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            int page = request.Page;
            int pagesize = request.PageSize;
            if (model.ExportID != Guid.Empty)
            {
                page = 1;
                pagesize = int.MaxValue - 1;
            }
            #region Load Data
            List<object> lstobj = new List<object>();
            lstobj.AddRange(new object[24]);
            lstobj[0] = model.OrgStructureID;
            lstobj[1] = model.WorkPlaceID;
            lstobj[2] = model.CodeEmp;
            lstobj[3] = model.ProfileName;
            lstobj[4] = model.ModelType;
            lstobj[5] = model.ModelName;
            lstobj[6] = model.ReceivePlaceID;
            lstobj[7] = model.StartDate;
            lstobj[8] = model.EndDate;
            lstobj[9] = model.ModelCode;
            lstobj[10] = EnumDropDown.StatusCheck.E_CHECKPASS.ToString();
            lstobj[11] = model.RequestFrom;
            lstobj[12] = model.RequestTo;
            lstobj[13] = model.IsReceive;
            lstobj[14] = model.IsLiquidated;
            lstobj[15] = model.Status;
            lstobj[16] = model.IsPaidDeposit;
            lstobj[17] = model.ReasonNotEligible;
            lstobj[18] = !string.IsNullOrEmpty(model.WorkPlaceIDs) ? Common.DotNetToOracle(model.WorkPlaceIDs) : null;
            lstobj[19] = model.EndDateFrom;
            lstobj[20] = model.EndDateTo;
            lstobj[21] = model.IsLiquitBefor;
            lstobj[22] = page;
            lstobj[23] = pagesize;
            var result = actionservice.GetData<Pur_MCAMEntity>(lstobj, ConstantSql.Hrm_Pur_SP_GET_PURMCAM, ref status).Translate<Pur_MCAMModel>().ToList();

            foreach (var item in result)
            {
                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.StatusCheck != null)
                {
                    item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKFAIL.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Fail.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKPASS.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Pass.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = ConstantDisplay.HRM_Category_Pur_MCAM_NotCheck.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                }

            }
            #endregion

            #region Xuất báo cáo
            if (model.ExportID != Guid.Empty)
            {
                var fullPath = ExportService.Export(model.ExportID, result, listHeaderInfo, UserGuidID, ExportFileType.Excel);
                return Json(fullPath);
            }
            #endregion
            var _datasource = result.ToDataSourceResult(request);
            int total = result.FirstOrDefault().GetPropertyValue("TotalRow") != null ? (int)result.FirstOrDefault().GetPropertyValue("TotalRow") : 0;
            _datasource.Total = total;
            _datasource.Data = result;
            return Json(_datasource, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// xuất work danh sách đủ điều kiện mua xe
        /// </summary>
        /// <param name="selectedIds"></param>
        /// <param name="valueFields"></param>
        /// <returns></returns>
        public ActionResult ExportListEnoughEligible(List<Guid> selectedIds, string valueFields)
        {
            #region tao ten file
            string messages = string.Empty;
            string status = string.Empty;
            DateTime DateStart = DateTime.Now;
            String suffix = DateStart.ToString("_ddMMyyyyHHmmss");
            string folferPath = string.Empty;
            string folderName = "ExportHre_Contract" + suffix;
            string dirpath = Common.GetPath(Common.DownloadURL);
            if (!Directory.Exists(dirpath))
                Directory.CreateDirectory(dirpath);
            if (selectedIds.Count() > 1)
            {
                folferPath = dirpath + "/" + folderName;
                Directory.CreateDirectory(folferPath);
            }
            else
            {
                folferPath = dirpath;
            }
            var fileDoc = string.Empty;
            #endregion

            #region Load template
            ActionService service = new ActionService(UserLogin, LanguageCode);
            var exportService = new Cat_ExportServices();
            Cat_ExportEntity template = null;
            string outputPath = string.Empty;
            List<object> lstObjExport = new List<object>();
            lstObjExport.Add(null);
            lstObjExport.Add(null);
            lstObjExport.Add(null);
            lstObjExport.Add(null);
            lstObjExport.Add(1);
            lstObjExport.Add(10000000);

            if (!string.IsNullOrEmpty(valueFields))
            {
                template = service.GetData<Cat_ExportEntity>(Guid.Parse(valueFields), ConstantSql.hrm_cat_sp_get_ExportById, ref status).FirstOrDefault();
            }

            if (template == null)
            {
                messages = "Error";
                return Json(messages, JsonRequestBehavior.AllowGet);
            }

            string templatepath = Common.GetPath(Common.TemplateURL + template.TemplateFile);

            if (!System.IO.File.Exists(templatepath))
            {
                messages = "NotTemplate";
                return Json(messages, JsonRequestBehavior.AllowGet);
            }

            if (!System.IO.File.Exists(templatepath))
            {
                messages = "NotTemplate";
                return Json(messages, JsonRequestBehavior.AllowGet);
            }
            int i = 0;
            #endregion
            var Service = new ActionService(UserLogin);
            string strids = string.Join(",", selectedIds);
            var lstPurCam = Service.GetData<Pur_MCAMEntity>(Common.DotNetToOracle(strids), ConstantSql.hrm_pur_sp_get_purmcambyids, ref status);
            if (lstPurCam == null)
                return null;
            foreach (var item in lstPurCam)
            {
                if (item.StartDate != null && item.LiquidationDate != null)
                {
                    item.CountMonth = (Convert.ToDouble((item.LiquidationDate - item.StartDate).ToString().Substring(0, (item.LiquidationDate - item.StartDate).ToString().IndexOf('.')))) / 30;
                }
                item.Day = DateStart.Day;
                item.Month = DateStart.Month;
                item.Year = DateStart.Year;
                outputPath = folferPath + "/" + Common.ChuyenTVKhongDau(item.ProfileName) + suffix + i.ToString() + "_" + template.TemplateFile;
                fileDoc = NotificationType.Success.ToString() + "," + Common.DownloadURL + "/" + Common.ChuyenTVKhongDau(item.ProfileName) + suffix + i.ToString() + "_" + template.TemplateFile;
                var lstMCAM = new List<Pur_MCAMEntity>();
                lstMCAM.Add(item);
                ExportService.ExportWord(outputPath, templatepath, lstMCAM);
            }
            if (lstPurCam.Count > 1)
            {
                var fileZip = Common.MultiExport("", true, folderName);
                return Json(fileZip);
            }
            return Json(fileDoc);
        }

        #endregion

        #region Pur_ProfileReceived
        public ActionResult GetLisProfileReceived([DataSourceRequest] DataSourceRequest request, Pur_MCAMSearchModel model)
        {
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            #region Header Info
            List<HeaderInfo> listHeaderInfo = new List<HeaderInfo>();
            HeaderInfo HeaderInfo1 = new HeaderInfo() { Name = "StartDate", Value = model.StartDate };
            HeaderInfo HeaderInfo2 = new HeaderInfo() { Name = "EndDate", Value = model.EndDate };
            HeaderInfo HeaderInfo3 = new HeaderInfo() { Name = "ProfileExport", Value = model.ProfileExport };
            HeaderInfo HeaderInfo4 = new HeaderInfo() { Name = "DateExport", Value = DateTime.Now };
            listHeaderInfo = new List<HeaderInfo>() { HeaderInfo1, HeaderInfo2, HeaderInfo3, HeaderInfo4 };

            #endregion
            #region Load template bao cao
            if (model != null && model.IsCreateTemplate)
            {
                var path = Common.GetPath("Templates");
                ExportService exportService = new ExportService();
                ConfigExport cfgExport = new ConfigExport()
                {
                    Object = new Pur_MCAMModel(),
                    FileName = "ListProfileReceived",
                    OutPutPath = path,
                    DownloadPath = "Templates",
                    HeaderInfo = listHeaderInfo,
                    IsDataTable = false

                };
                var str = exportService.CreateTemplate(cfgExport);
                return Json(str);
            }
            #endregion
            #region phân trang
            int Page = request.Page;
            int PageSize = request.PageSize;
            if (model.ExportID != Guid.Empty)
            {
                Page = 1;
                PageSize = int.MaxValue - 1;
            }
            #endregion
            #region Load Danh sach nhan vien da nhan xe
            model.IsReceive = true;
            #region kiem tra thanh ly hay chua
            if (!string.IsNullOrEmpty(model.StrLiquidated))
            {
                if (model.StrLiquidated == EnumDropDown.ISLIQUIDATED.E_ISLIQUIDATED.ToString())
                {
                    model.IsLiquidated = true;
                }
                else
                {
                    model.IsLiquidated = false;
                }
            }
            else
            {
                model.IsLiquidated = null;
            }
            #endregion

            List<object> lstobj = new List<object>();
            lstobj.AddRange(new object[24]);
            lstobj[0] = model.OrgStructureID;
            lstobj[1] = model.WorkPlaceID;
            lstobj[2] = model.CodeEmp;
            lstobj[3] = model.ProfileName;
            lstobj[4] = model.ModelType;
            lstobj[5] = model.ModelName;
            lstobj[6] = model.ReceivePlaceID;
            lstobj[7] = model.StartDate;
            lstobj[8] = model.EndDate;
            lstobj[9] = model.ModelCode;
            lstobj[10] = EnumDropDown.StatusCheck.E_CHECKPASS.ToString();
            lstobj[11] = model.RequestFrom;
            lstobj[12] = model.RequestTo;
            lstobj[13] = model.IsReceive;
            lstobj[14] = model.IsLiquidated;
            lstobj[15] = EnumDropDown.ModelStatus.E_APPROVED.ToString();
            lstobj[16] = model.IsPaidDeposit;
            lstobj[17] = model.ReasonNotEligible;
            lstobj[18] = !string.IsNullOrEmpty(model.WorkPlaceIDs) ? Common.DotNetToOracle(model.WorkPlaceIDs) : null;
            lstobj[19] = model.EndDateFrom;
            lstobj[20] = model.EndDateTo;
            lstobj[21] = model.IsLiquitBefor;
            lstobj[22] = Page;
            lstobj[23] = PageSize;
            var result = actionservice.GetData<Pur_MCAMEntity>(lstobj, ConstantSql.Hrm_Pur_SP_GET_PURMCAM, ref status).Translate<Pur_MCAMModel>().ToList();

            foreach (var item in result)
            {
                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                }

            }
            #endregion

            if (model.ExportID != Guid.Empty)
            {
                var fullPath = ExportService.Export(model.ExportID, result, listHeaderInfo, UserGuidID, ExportFileType.Excel);
                return Json(fullPath);
            }
            var _datasource = result.ToDataSourceResult(request);
            int total = result.FirstOrDefault().GetPropertyValue("TotalRow") != null ? (int)result.FirstOrDefault().GetPropertyValue("TotalRow") : 0;
            _datasource.Total = total;
            _datasource.Data = result;
            return Json(_datasource, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// xuất báo cáo nhân viên đã nhận xe
        /// </summary>
        /// <param name="selectedIds"></param>
        /// <param name="valueFields"></param>
        /// <returns></returns>
        public ActionResult ExportSelectProfileReceived(string selectedIds, string valueFields)
        {
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            var result = actionservice.GetData<Pur_MCAMEntity>(Common.DotNetToOracle(selectedIds), ConstantSql.hrm_pur_sp_get_purmcambyids, ref status).Translate<Pur_MCAMModel>().ToList();
            //Lấy Những dòng IsReceive = true.
            result = result.Where(s => s.IsReceive != null && s.IsReceive == true).ToList();

            foreach (var item in result)
            {

                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                    if (item.ModelType != null)
                        item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
            }
            #region Load template bao cao
            if (result != null && status == NotificationType.Success.ToString())
            {
                status = ExportService.Export(Guid.Empty, result, valueFields.Split(','), null);
            }
            #endregion
            return Json(status);
        }


        public ActionResult ExportAllProfileReceived([DataSourceRequest] DataSourceRequest request, Pur_MCAMSearchModel model)
        {
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;

            #region Load Danh sach nhan vien da nhan xe va loai bo nhung dong trung nhau
            //Đã nhận xe.
            model.IsReceive = true;
            List<object> lstobj = new List<object>();
            lstobj.AddRange(new object[24]);
            lstobj[0] = model.OrgStructureID;
            lstobj[1] = model.WorkPlaceID;
            lstobj[2] = model.CodeEmp;
            lstobj[3] = model.ProfileName;
            lstobj[4] = model.ModelType;
            lstobj[5] = model.ModelName;
            lstobj[6] = model.ReceivePlaceID;
            lstobj[7] = model.StartDate;
            lstobj[8] = model.EndDate;
            lstobj[9] = model.ModelCode;
            lstobj[10] = model.StatusCheck;
            lstobj[11] = model.RequestFrom;
            lstobj[12] = model.RequestTo;
            lstobj[13] = model.IsReceive;
            lstobj[14] = model.IsLiquidated;
            lstobj[15] = model.Status;
            lstobj[16] = model.IsPaidDeposit;
            lstobj[17] = model.ReasonNotEligible;
            lstobj[18] = !string.IsNullOrEmpty(model.WorkPlaceIDs) ? Common.DotNetToOracle(model.WorkPlaceIDs) : null;
            lstobj[19] = model.EndDateFrom;
            lstobj[20] = model.EndDateTo;
            lstobj[21] = model.IsLiquitBefor;
            lstobj[22] = 1;
            lstobj[23] = int.MaxValue - 1;
            var result = actionservice.GetData<Pur_MCAMEntity>(lstobj, ConstantSql.Hrm_Pur_SP_GET_PURMCAM, ref status).Translate<Pur_MCAMModel>().ToList();


            foreach (var item in result)
            {
                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                }
            }
            #endregion
            if (result != null && status == NotificationType.Success.ToString())
            {
                status = ExportService.Export(result, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));
            }
            return Json(status);
        }


        #endregion

        #region Pur_MCAM (đăng ký mua xe)

        /// <summary>
        /// Xuất báo cáo những phần tử được chọn
        /// </summary>
        /// <param name="selectedIds"></param>
        /// <param name="valueFields"></param>
        /// <returns></returns>
        public ActionResult ExportSelectedListPurMCAM(string selectedIds, string valueFields)
        {
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                #region Load du lieu
                var ListID = selectedIds.Split(',').ToList();
                var result = new List<Pur_MCAMModel>();
                foreach (var item in ListID.Chunk(100))
                {
                    var listIDs = string.Join(",", item);
                    var resultbyitem = actionservice.GetData<Pur_MCAMEntity>(Common.DotNetToOracle(listIDs), ConstantSql.hrm_pur_sp_get_purmcambyids, ref status).Translate<Pur_MCAMModel>().ToList();
                    if (!Common.CheckListNullOrEmty(resultbyitem))
                        result.AddRange(resultbyitem);
                }

                #endregion
                #region CatNameEnntity_lý do kho đủ điều kiện
                var objNameEntity = new List<object>();
                objNameEntity.AddRange(new object[4]);
                objNameEntity[1] = EnumDropDown.EntityType.E_ReasonNotEligible.ToString();
                objNameEntity[2] = 1;
                objNameEntity[3] = Int32.MaxValue - 1;
                var ListNameEntity = actionservice.GetData<Cat_NameEntityEntity>(objNameEntity, ConstantSql.hrm_cat_sp_get_NameEntity, ref status);
                #endregion
                #region Kiểm tra trùng và dịch enum
                foreach (var item in result)
                {
                    if (item.ModelType != null)
                    {
                        item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                    }
                    if (item.StatusCheck != null)
                    {
                        item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKFAIL.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Fail.TranslateString() : string.Empty;
                        if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                            item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKPASS.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Pass.TranslateString() : string.Empty;
                        if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                            item.StatusCheckTranslate = ConstantDisplay.HRM_Category_Pur_MCAM_NotCheck.TranslateString();
                    }
                    if (item.Status != null)
                    {
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                        if (string.IsNullOrEmpty(item.StatusTranslate))
                            item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                        if (string.IsNullOrEmpty(item.StatusTranslate))
                            item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                    }
                    if (item.ReasonNotEligible != null)
                    {
                        var _src = ListNameEntity.Where(s => s.Code == item.ReasonNotEligible).FirstOrDefault();
                        item.ReasonNotEligibleTranslate = _src.Description;
                    }
                }
                #endregion

                #region Load template bao cao
                if (result != null && status == NotificationType.Success.ToString())
                {
                    status = ExportService.Export(Guid.Empty, result, valueFields.Split(','), null);
                }
                #endregion
                return Json(status);
            }
            return null;
        }
        /// <summary>
        /// Xuất báo cáo tất cả
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult ExportAllListPurMCAM([DataSourceRequest] DataSourceRequest request, Pur_MCAMSearchModel model)
        {
            model.SetPropertyValue("IsExport", true);
            #region Load du lieu
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            List<object> lstobj = new List<object>();
            lstobj.AddRange(new object[24]);
            lstobj[0] = model.OrgStructureID;
            lstobj[1] = model.WorkPlaceID;
            lstobj[2] = model.CodeEmp;
            lstobj[3] = model.ProfileName;
            lstobj[4] = model.ModelType;
            lstobj[5] = model.ModelName;
            lstobj[6] = model.ReceivePlaceID;
            lstobj[7] = model.StartDate;
            lstobj[8] = model.EndDate;
            lstobj[9] = model.ModelCode;
            lstobj[10] = model.StatusCheck;
            lstobj[11] = model.RequestFrom;
            lstobj[12] = model.RequestTo;
            lstobj[13] = model.IsReceive;
            lstobj[14] = model.IsLiquidated;
            lstobj[15] = model.Status;
            lstobj[16] = model.IsPaidDeposit;
            lstobj[17] = model.ReasonNotEligible;
            lstobj[18] = !string.IsNullOrEmpty(model.WorkPlaceIDs) ? Common.DotNetToOracle(model.WorkPlaceIDs) : null;
            lstobj[19] = model.EndDateFrom;
            lstobj[20] = model.EndDateTo;
            lstobj[21] = model.IsLiquitBefor;
            lstobj[22] = 1;
            lstobj[23] = int.MaxValue - 1;
            var result = actionservice.GetData<Pur_MCAMEntity>(lstobj, ConstantSql.Hrm_Pur_SP_GET_PURMCAM, ref status).Translate<Pur_MCAMModel>().ToList();
            #endregion
            #region CatNameEnntity_lý do kho đủ điều kiện
            var objNameEntity = new List<object>();
            objNameEntity.AddRange(new object[4]);
            objNameEntity[1] = EnumDropDown.EntityType.E_ReasonNotEligible.ToString();
            objNameEntity[2] = 1;
            objNameEntity[3] = Int32.MaxValue - 1;
            var ListNameEntity = actionservice.GetData<Cat_NameEntityEntity>(objNameEntity, ConstantSql.hrm_cat_sp_get_NameEntity, ref status);
            #endregion

            #region Kiểm tra trùng và dịch enum
            foreach (var item in result)
            {
                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.StatusCheck != null)
                {
                    item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKFAIL.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Fail.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKPASS.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Pass.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = ConstantDisplay.HRM_Category_Pur_MCAM_NotCheck.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                }
                if (item.ReasonNotEligible != null)
                {
                    var _src = ListNameEntity.Where(s => s.Code == item.ReasonNotEligible).FirstOrDefault();
                    item.ReasonNotEligibleTranslate = _src.Description;
                }
            }
            #endregion

            if (status == NotificationType.Success.ToString())
            {
                status = ExportService.Export(result, model.GetPropertyValue("ValueFields").TryGetValue<string>().Split(','));
            }
            return Json(status);
        }
        #region
        /// <summary>
        /// Load đăng ký mua xe
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetListPurMCAM([DataSourceRequest] DataSourceRequest request, Pur_MCAMSearchModel model)
        {
            #region Load du lieu
            ActionService actionservice = new ActionService(UserLogin);
            string status = string.Empty;
            List<object> lstobj = new List<object>();
            lstobj.AddRange(new object[24]);
            lstobj[0] = model.OrgStructureID;
            lstobj[1] = model.WorkPlaceID;
            lstobj[2] = model.CodeEmp;
            lstobj[3] = model.ProfileName;
            lstobj[4] = model.ModelType;
            lstobj[5] = model.ModelName;
            lstobj[6] = model.ReceivePlaceID;
            lstobj[7] = model.StartDate;
            lstobj[8] = model.EndDate;
            lstobj[9] = model.ModelCode;
            lstobj[10] = model.StatusCheck;
            lstobj[11] = model.RequestFrom;
            lstobj[12] = model.RequestTo;
            lstobj[13] = model.IsReceive;
            lstobj[14] = model.IsLiquidated;
            lstobj[15] = model.Status;
            lstobj[16] = model.IsPaidDeposit;
            lstobj[17] = model.ReasonNotEligible;
            lstobj[18] = !string.IsNullOrEmpty(model.WorkPlaceIDs) ? Common.DotNetToOracle(model.WorkPlaceIDs) : null;
            lstobj[19] = model.EndDateFrom;
            lstobj[20] = model.EndDateTo;
            lstobj[21] = model.IsLiquitBefor;
            lstobj[22] = request.Page;
            lstobj[23] = request.PageSize;
            var result = actionservice.GetData<Pur_MCAMEntity>(lstobj, ConstantSql.Hrm_Pur_SP_GET_PURMCAM, ref status).Translate<Pur_MCAMModel>().ToList();
            #endregion
            #region reasonnoteligiable
            var objNameEntity = new List<object>();
            objNameEntity.AddRange(new object[4]);
            objNameEntity[1] = EnumDropDown.EntityType.E_ReasonNotEligible.ToString();
            objNameEntity[2] = 1;
            objNameEntity[3] = Int32.MaxValue - 1;
            var ListNameEntity = actionservice.GetData<Cat_NameEntityEntity>(objNameEntity, ConstantSql.hrm_cat_sp_get_NameEntity, ref status);
            #endregion
            #region Lương và hợp đồng
            List<string> ListProfileID = new List<string>();
            if (result != null)
            {
                ListProfileID = result.Where(s => s.ProfileID != null).Select(s => s.ProfileID.ToString()).Distinct().ToList();
            }
            string strIDs = string.Empty;
            var _ListcontractEntity = new List<Hre_ContractSmallEntity>();
            var _listbasicSalaryEntity = new List<Sal_BasicSalarySmallEntity>();
            if (ListProfileID != null && ListProfileID.Count() > 0)
            {
                strIDs = string.Join(",", ListProfileID);
                _ListcontractEntity = actionservice.GetData<Hre_ContractSmallEntity>(Common.DotNetToOracle(strIDs), ConstantSql.hrm_hr_sp_get_contractbyproidsv2, ref status);
                _listbasicSalaryEntity = actionservice.GetData<Sal_BasicSalarySmallEntity>(Common.DotNetToOracle(strIDs), ConstantSql.hrm_sal_sp_get_BasicSalaryByProfileIdsv2, ref status);
            }
            #endregion
            #region dịch enum
            foreach (var item in result)
            {
                if (item.ModelType != null)
                {
                    item.ModelTypeTranslate = item.ModelType == EnumDropDown.ModelType.E_CAR.ToString() ? ConstantDisplay.HRM_Cat_ModelType_Car.TranslateString() : ConstantDisplay.HRM_Cat_ModelType_Motor.TranslateString();
                }
                if (item.StatusCheck != null)
                {
                    item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKFAIL.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Fail.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = item.StatusCheck == EnumDropDown.StatusCheck.E_CHECKPASS.ToString() ? ConstantDisplay.HRM_Category_Pur_MCAM_Pass.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusCheckTranslate))
                        item.StatusCheckTranslate = ConstantDisplay.HRM_Category_Pur_MCAM_NotCheck.TranslateString();
                }
                if (item.Status != null)
                {
                    item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_APPROVED.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_APPROVED.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_WAITING.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_WAITING.TranslateString() : string.Empty;
                    if (string.IsNullOrEmpty(item.StatusTranslate))
                        item.StatusTranslate = item.Status == EnumDropDown.ModelStatus.E_CANCEL.ToString() ? ConstantDisplay.HRM_StatusTranslate_E_CANCEL.TranslateString() : string.Empty;
                }
                if (_ListcontractEntity != null)
                {
                    var contract = _ListcontractEntity.Where(s => s.ProfileID == item.ProfileID).OrderByDescending(s => s.DateStart).FirstOrDefault();
                    if (contract != null)
                        item.Cat_contractType = contract.ContractTypeName;
                }
                if (_listbasicSalaryEntity != null)
                {
                    var salary = _listbasicSalaryEntity.Where(s => s.ProfileID == item.ProfileID && s.Status == EnumDropDown.Sal_BasicSalaryStatus.E_APPROVED.ToString()).OrderByDescending(s => s.DateOfEffect).FirstOrDefault();
                    if (salary != null)
                        item.GrossAmount = salary.GrossAmount.ToDouble();
                }
                if (item.ReasonNotEligible != null)
                {
                    var _src = ListNameEntity.Where(s => s.Code == item.ReasonNotEligible).FirstOrDefault();
                    if (_src != null)
                    {
                        item.ReasonNotEligibleTranslate = _src.NameEntityName;
                    }
                }
            }
            #endregion
            var _datasource = result.ToDataSourceResult(request);
            int total = result.FirstOrDefault().GetPropertyValue("TotalRow") != null ? (int)result.FirstOrDefault().GetPropertyValue("TotalRow") : 0;
            _datasource.Total = total;
            _datasource.Data = result;
            return Json(_datasource, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cat_MasterDataGroup
        #region Cat_SysLockObject
        public ActionResult ExportSysLockObjectAll([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return ExportAllAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LockObject);
        }
        public ActionResult ExportSysLockObjectbyIDs(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LockObjectByIDs);
        }
        #endregion
        public JsonResult GetMultiCatTable(string text)
        {
            var service = new BaseService();
            string status = "";
            List<object> tableParam = new List<object>();
            tableParam.AddRange(new object[3]);
            tableParam[0] = text;
            tableParam[1] = 1;
            tableParam[2] = Int32.MaxValue - 1;
            var result = service.GetData<Cat_SysTablesMultiEntity>(tableParam, ConstantSql.hrm_cat_sp_get_tables, UserLogin, ref status);
            result = result.Where(p => p.Name.Substring(0, 3) == HRM.Infrastructure.Utilities.ModuleKey.Cat.ToString()).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiCatTableForDashBoard(Guid userID)
        {
            var service = new Sys_UserServices();
            var lstMasterDataGroupMulti = service.GetMultiCatTableForDashBoard(userID);
            return Json(lstMasterDataGroupMulti, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChildObjectName(string objectName)
        {
            var services = new Cat_MasterDataGroupServices();
            var result = services.GetChildItems(objectName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Cat_Skill
        public ActionResult GetSkillList([DataSourceRequest] DataSourceRequest request, Cat_SkillSearchModel model)
        {
            return GetListDataAndReturn<Cat_SkillModel, Cat_SkillEntity, Cat_SkillSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Skill);
        }
        public ActionResult ExportSkillSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SkillEntity, Cat_SkillModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SkillByIds);
        }
        public ActionResult ExportAllSkillList([DataSourceRequest] DataSourceRequest request, Cat_SkillSearchModel model)
        {
            return ExportAllAndReturn<Cat_SkillEntity, Cat_SkillModel, Cat_SkillSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Skill);
        }
        public ActionResult GetSkillTopicBySkillID([DataSourceRequest] DataSourceRequest request, Guid skillID)
        {
            string status = string.Empty;

            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(skillID);
            objs.Add(request.Page);
            objs.Add(request.PageSize);
            var result = baseService.GetData<Cat_SkillTopicModel>(objs, ConstantSql.hrm_cat_sp_get_SkillTopicBySkillId, UserLogin, ref status);
            if (result != null)

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return null;
        }

        #endregion

        public ActionResult CreateAndUpdateOrtherInfoOrg([Bind]Cat_OtherInfoOrgModel model)
        {
            var status = string.Empty;
            var message = string.Empty;
            var baseService = new BaseService();
            var orgInfoServices = new Cat_OrgMoreInforServices();
            var orgModel = new Cat_OtherInfoOrgModel();
            if (model != null && model.OrgstructureID != null)
            {
                var objs = new List<object>();
                objs.Add(Common.DotNetToOracle(model.OrgstructureID.Value.ToString()));
                var orgInfoEntity = baseService.GetData<Cat_OtherInfoOrgEntity>(objs, ConstantSql.hrm_Cat_sp_get_OtherInfoOrgByOrgID, UserLogin, ref status).FirstOrDefault();
                if (orgInfoEntity != null)
                {
                    orgInfoEntity.OrgstructureID = model.OrgstructureID;
                    if (!model._taginfo)
                    {
                        orgInfoEntity.Min1 = model.Min1;
                        orgInfoEntity.Min2 = model.Min2;
                        orgInfoEntity.MLC1 = model.MLC1;
                        orgInfoEntity.MLC2 = model.MLC1;
                        for (int i = 1; i <= 20; i++)
                        {
                            string ppt_Postion = string.Format("PositionID{0}", i.ToString());
                            string ppt_Count = string.Format("Count{0}", i.ToString());
                            orgInfoEntity.SetPropertyValue(ppt_Postion, model.GetPropertyValue(ppt_Postion));
                            orgInfoEntity.SetPropertyValue(ppt_Count, model.GetPropertyValue(ppt_Count));
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 16; i++)
                        {
                            string ppt_Info = string.Format("Info{0}", i.ToString());
                            orgInfoEntity.SetPropertyValue(ppt_Info, model.GetPropertyValue(ppt_Info));
                        }
                    }
                    message = orgInfoServices.Edit(orgInfoEntity);
                    orgModel = orgInfoEntity.CopyData<Cat_OtherInfoOrgModel>();
                    orgModel.ActionStatus = message;

                }
                else
                {
                    var orgInfoAddEntity = new Cat_OtherInfoOrgEntity();
                    orgInfoAddEntity.OrgstructureID = model.OrgstructureID;
                    if (!model._taginfo)
                    {
                        orgInfoAddEntity.Min1 = model.Min1;
                        orgInfoAddEntity.Min2 = model.Min2;
                        orgInfoAddEntity.MLC1 = model.MLC1;
                        orgInfoAddEntity.MLC2 = model.MLC1;
                        for (int i = 1; i <= 20; i++)
                        {
                            string ppt_Postion = string.Format("PositionID{0}", i.ToString());
                            string ppt_Count = string.Format("Count{0}", i.ToString());
                            orgInfoAddEntity.SetPropertyValue(ppt_Postion, model.GetPropertyValue(ppt_Postion));
                            orgInfoAddEntity.SetPropertyValue(ppt_Count, model.GetPropertyValue(ppt_Count));
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 16; i++)
                        {
                            string ppt_Info = string.Format("Info{0}", i.ToString());
                            orgInfoAddEntity.SetPropertyValue(ppt_Info, model.GetPropertyValue(ppt_Info));
                        }
                    }
                    message = orgInfoServices.Add(orgInfoAddEntity);
                    orgModel = orgInfoAddEntity.CopyData<Cat_OtherInfoOrgModel>();
                    orgModel.ActionStatus = message;
                }
            }

            return Json(orgModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAndUpdateOrgMoreInfo(Guid? orgID, string servicesType, DateTime? contractFrom, DateTime? contractTo,
           string billingCompanyName, string billingAddress, string taxCode, string des, string durationPay, string recipientInvoice,
           string tetePhone, string cellPhone, string email, string mailingAddress, string freeService, string codeOrg)
        {
            var status = string.Empty;
            var message = string.Empty;
            var baseService = new BaseService();
            var orgInfoServices = new Cat_OrgMoreInforServices();
            var orgModel = new Cat_OrgMoreInforModel();
            if (contractFrom > contractTo)
            {
                orgModel.ActionStatus = ConstantDisplay.HRM_Cat_OrgInfo_DateEndMustLaterThanDateStart.TranslateString();
                return Json(orgModel, JsonRequestBehavior.AllowGet);
            }
            if (orgID != null)
            {
                var objs = new List<object>();
                objs.Add(Common.DotNetToOracle(orgID.Value.ToString()));
                var orgInfoEntity = baseService.GetData<Cat_OrgMoreInforEntity>(objs, ConstantSql.hrm_hr_sp_get_OrgMoreInfoByOrgID, UserLogin, ref status).FirstOrDefault();
                if (orgInfoEntity != null)
                {
                    orgInfoEntity.ServicesType = servicesType;
                    orgInfoEntity.ContractFrom = contractFrom;
                    orgInfoEntity.ContractTo = contractTo;
                    orgInfoEntity.BillingCompanyName = billingCompanyName;
                    orgInfoEntity.BillingAddress = billingAddress;
                    orgInfoEntity.TaxCode = taxCode;
                    orgInfoEntity.Description = des;
                    orgInfoEntity.DurationPay = durationPay;
                    orgInfoEntity.RecipientInvoice = recipientInvoice;
                    orgInfoEntity.TelePhone = tetePhone;
                    orgInfoEntity.CellPhone = cellPhone;
                    orgInfoEntity.Email = email;
                    orgInfoEntity.OrgStructureID = orgID;
                    orgInfoEntity.FreeService = freeService;
                    orgInfoEntity.MailingAddress = mailingAddress;
                    orgInfoEntity.CodeOrgStructure = codeOrg;
                    message = orgInfoServices.Edit(orgInfoEntity);
                    orgModel = orgInfoEntity.CopyData<Cat_OrgMoreInforModel>();
                    orgModel.ActionStatus = message;

                }
                else
                {
                    var orgInfoAddEntity = new Cat_OrgMoreInforEntity();
                    orgInfoAddEntity.ServicesType = servicesType;
                    orgInfoAddEntity.ContractFrom = contractFrom;
                    orgInfoAddEntity.ContractTo = contractTo;
                    orgInfoAddEntity.BillingCompanyName = billingCompanyName;
                    orgInfoAddEntity.BillingAddress = billingAddress;
                    orgInfoAddEntity.TaxCode = taxCode;
                    orgInfoAddEntity.Description = des;
                    orgInfoAddEntity.DurationPay = durationPay;
                    orgInfoAddEntity.RecipientInvoice = recipientInvoice;
                    orgInfoAddEntity.TelePhone = tetePhone;
                    orgInfoAddEntity.CellPhone = cellPhone;
                    orgInfoAddEntity.Email = email;
                    orgInfoAddEntity.OrgStructureID = orgID;
                    orgInfoAddEntity.FreeService = freeService;
                    orgInfoAddEntity.MailingAddress = mailingAddress;
                    orgInfoAddEntity.CodeOrgStructure = codeOrg;
                    message = orgInfoServices.Add(orgInfoAddEntity);
                    orgModel = orgInfoAddEntity.CopyData<Cat_OrgMoreInforModel>();
                    orgModel.ActionStatus = message;
                }
            }

            return Json(orgModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedTravelRequest(Guid travelRequestId, Guid userId, string type)
        {
            var status = string.Empty;
            var message = string.Empty;

            var services = new FIN_ClaimService();
            message = services.ApprovedTravelRequest(travelRequestId, userId, type, UserLogin, Guid.Empty);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedCashAdvance(Guid cashAdvanceId, Guid userId, string type)
        {
            var status = string.Empty;
            var message = string.Empty;

            var services = new FIN_CashAdvanceService();
            message = services.ApprovedCashAdvance(cashAdvanceId, userId, type, UserLogin, Guid.Empty);

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedClaim(Guid claimId, Guid userId, string type)
        {
            var status = string.Empty;
            var message = string.Empty;

            var services = new FIN_ClaimService();
            message = services.ApprovedClaim(claimId, userId, type, UserLogin, Guid.Empty);

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        // lấy ds nguồn chi phí
        public ActionResult GetMultiCostSource(string text)
        {
            return GetDataForControl<CatNameEntityModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_CostSource_Multi);
        }

        // lấy ds phương tiện đi làm
        public ActionResult GetMultiVehicle(string text)
        {
            return GetDataForControl<CatNameEntityModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Vehicle_Multi);
        }

        #region Cat_AbilitiTitle
        [HttpPost]
        public ActionResult GetAbilityTileList([DataSourceRequest] DataSourceRequest request, Cat_AbilityTileSearchModel model)
        {
            return GetListDataAndReturn<Cat_AbilityTileModel, Cat_AbilityTileEntity, Cat_AbilityTileSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AbilityTile);
        }

        public ActionResult ExportAllAbilityTileList([DataSourceRequest] DataSourceRequest request, Cat_AbilityTileSearchModel model)
        {
            return ExportAllAndReturn<Cat_AbilityTileEntity, Cat_AbilityTileModel, Cat_AbilityTileSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_AbilityTile);
        }

        public ActionResult GetMultiAbilityTile(string text)
        {
            return GetDataForControl<Cat_AbilityTileModel, Cat_AbilityTileEntity>(text, ConstantSql.hrm_cat_sp_get_AbilityTile_Multi);
        }

        public ActionResult GetMultiAbilityTileVNI(string text)
        {
            return GetDataForControl<Cat_AbilityTileModel, Cat_AbilityTileEntity>(text, ConstantSql.hrm_cat_sp_get_AbilityTile_Multi);
        }

        #endregion

        #region
        [HttpPost]
        public ActionResult GetSurveyList([DataSourceRequest] DataSourceRequest request, Cat_SurveySearchModel model)
        {
            return GetListDataAndReturn<Cat_SurveyModel, Cat_SurveyEntity, Cat_SurveySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Survey);
        }
        #endregion
        [HttpPost]
        public ActionResult SurveyQuestionTypeList([DataSourceRequest] DataSourceRequest request, Cat_SurveyQuestionTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_SurveyQuestionTypeModel, Cat_SurveyQuestionTypeEntity, Cat_SurveyQuestionTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SurveyQuestionType);
        }

        #region Hien.Nguyen TimeAnalyze - HoldSalary

        public ActionResult GetTimeAnalyze_CatNameEntity(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_TimeAnalyze);
        }

        #endregion

        public JsonResult GetReqDocumentByResignReasonID(Guid ID)
        {
            if (ID != Guid.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                List<object> listModel = new List<object>();
                listModel.AddRange(new object[4]);
                listModel[2] = 1;
                listModel[3] = int.MaxValue - 1;
                var listReqDocument = baseService.GetData<Cat_ReqDocumentEntity>(listModel, ConstantSql.hrm_cat_sp_get_ReqDocument, UserLogin, ref status);
                if (listReqDocument.Count == 0)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    listReqDocument = listReqDocument.Where(s => s.ResignReasonID == ID).ToList();
                    var result = from e in listReqDocument
                                 select new
                                 {
                                     Text = e.ReqDocumentName.TranslateString(),
                                     Value = e.ID,
                                 };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReqDocumentCascading(Guid ReqDocumentID, string ReqDocumentFilter)
        {
            var result = new List<Cat_ReqDocumentModel>();
            if (ReqDocumentID != Guid.Empty)
            {
                string status = string.Empty;
                var baseService = new BaseService();
                var service = new Cat_ProvinceServices();
                List<object> listModel = new List<object>();
                listModel.AddRange(new object[4]);
                listModel[2] = 1;
                listModel[3] = int.MaxValue - 1;
                var listReqDocument = baseService.GetData<Cat_ReqDocumentModel>(listModel, ConstantSql.hrm_cat_sp_get_ReqDocument, UserLogin, ref status);
                result = listReqDocument.Where(s => s.ResignReasonID == ReqDocumentID).ToList();
                if (!string.IsNullOrEmpty(ReqDocumentFilter))
                {
                    var rs = result.Where(s => s.ReqDocumentName != null && s.ReqDocumentName.ToLower().Contains(ReqDocumentFilter.ToLower())).ToList();
                    rs = rs.OrderBy(s => s.ReqDocumentName).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Cat_Sync
        [HttpPost]
        public ActionResult GetCatSync([DataSourceRequest] DataSourceRequest request, Cat_SyncSearchModel model)
        {
            return GetListDataAndReturn<Cat_SyncEntity, Cat_SyncEntity, Cat_SyncSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Sync);
        }
        #endregion

        #region Bảo Hiểm Tự Nguyện

        public ActionResult ExportAllVoluntaryInsTypeList([DataSourceRequest] DataSourceRequest request, Cat_VoluntaryInsTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_VoluntaryInsTypeEntity, Cat_VoluntaryInsTypeModel, Cat_VoluntaryInsTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_VoluntaryInsType);
        }

        public ActionResult ExportVoluntaryInsTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_VoluntaryInsTypeEntity, Cat_VoluntaryInsTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_VoluntaryInsTypeByIds);
        }

        public ActionResult GetCatVoluntaryInsTypeList([DataSourceRequest] DataSourceRequest request, Cat_VoluntaryInsTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_VoluntaryInsTypeEntity, Cat_VoluntaryInsTypeEntity, Cat_VoluntaryInsTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_VoluntaryInsType);
        }

        public ActionResult GetCatVoluntaryInsCompanyList([DataSourceRequest] DataSourceRequest request, Cat_VoluntaryInsCompanySearchModel model)
        {
            return GetListDataAndReturn<Cat_VoluntaryInsCompanyEntity, Cat_VoluntaryInsCompanyEntity, Cat_VoluntaryInsCompanySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_VoluntaryInsCompany);
        }
        public ActionResult ExportVoluntaryInsCompanySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_VoluntaryInsCompanyEntity, Cat_VoluntaryInsCompanyModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_VoluntaryInsCompanyByIds);
        }
        public ActionResult ExportVoluntaryInsCompany([DataSourceRequest] DataSourceRequest request, Cat_VoluntaryInsCompanySearchModel model)
        {
            return ExportAllAndReturn<Cat_VoluntaryInsCompanyEntity, Cat_VoluntaryInsCompanyEntity, Cat_VoluntaryInsCompanySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_VoluntaryInsCompany);
        }
        #endregion
        #region Chế độ BH cho NV
        public ActionResult GetIns_InsuranceGradeList([DataSourceRequest] DataSourceRequest request, Ins_InsuranceGradeModelSearch model)
        {
            return GetListDataAndReturn<Ins_InsuranceGradeEntity, Ins_InsuranceGradeEntity, Ins_InsuranceGradeModelSearch>(request, model, ConstantSql.hrm_ins_sp_get_InsuranceGrade);
        }
        public ActionResult ExportIns_InsuranceGradeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Ins_InsuranceGradeEntity, Ins_InsuranceGradeEntity>(selectedIds, valueFields, ConstantSql.hrm_ins_sp_get_ins_sp_get_InsuranceGradeByIds);
        }
        public ActionResult ExportAllIns_InsuranceGradeList([DataSourceRequest] DataSourceRequest request, Ins_InsuranceGradeModelSearch model)
        {
            return ExportAllAndReturn<Ins_InsuranceGradeEntity, Ins_InsuranceGradeEntity, Ins_InsuranceGradeModelSearch>(request, model, ConstantSql.hrm_ins_sp_get_InsuranceGrade);
        }
        #endregion

        #region Cat_EvaluationResult
        [HttpPost]
        public ActionResult GetEvaluationResultList([DataSourceRequest] DataSourceRequest request, Cat_EvaluationResultSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_EvaluationResultSearchModel>(request, model, ConstantSql.cat_sp_get_EvaluationResult);
        }

        public JsonResult GetMultiEvaluationResult(string text)
        {
            return GetDataForControl<CatNameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.cat_sp_get_EvaluationResult_Multi);
        }

        public ActionResult ExportAllEvaluationResultList([DataSourceRequest] DataSourceRequest request, Cat_EvaluationResultSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_EvaluationResultSearchModel>(request, model, ConstantSql.cat_sp_get_EvaluationResult);
        }

        public JsonResult GetNameEntityByCode(string code)
        {
            string status = string.Empty;
            var actionService = new ActionService(UserLogin);
            List<object> listModel = new List<object>();
            listModel.AddRange(new object[4]);
            listModel[1] = code;
            listModel[2] = 1;
            listModel[3] = int.MaxValue - 1;
            var result = baseService.GetData<Cat_NameEntityModel>(listModel, ConstantSql.cat_sp_get_EvaluationResult, UserLogin, ref status).FirstOrDefault();
            if (result != null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null);
            }
        }
        #endregion

        #region Cat_shiftPrice
        public ActionResult GetshiftPriceList([DataSourceRequest] DataSourceRequest request, Cat_shiftPriceSearchModel model)
        {
            return GetListDataAndReturn<Cat_shiftPriceModel, Cat_shiftPriceEntity, Cat_shiftPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_shiftPrice);
        }
        public ActionResult ExportshiftPriceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_shiftPriceEntity, Cat_shiftPriceModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_shiftPriceByIds);
        }
        public ActionResult ExportAllshiftPriceList([DataSourceRequest] DataSourceRequest request, Cat_shiftPriceSearchModel model)
        {
            return ExportAllAndReturn<Cat_shiftPriceEntity, Cat_shiftPriceModel, Cat_shiftPriceSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_shiftPrice);
        }
        #endregion

        public JsonResult GetMultiImportAtt(string text)
        {
            return GetDataForControl<Cat_ImportAttMultiModel, Cat_ImportAttEntity>(text, ConstantSql.hrm_cat_sp_get_ImportAtt_Multi);
        }
        public ActionResult GetUnusualAllowanceCfgAmount(Guid? UnusualAllowanceID, DateTime? Month)
        {
            if (Month != null && UnusualAllowanceID != null)
            {
                //return GetListDataAndReturn<Cat_UnAllowCfgAmountModel, Cat_UnAllowCfgAmountEntity, Cat_UnAllowCfgAmountSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_UnAllowCfgAmount);
                string status = string.Empty;
                var baseService = new BaseService();
                List<object> listModel = new List<object>();
                listModel = Common.AddRange(5);
                listModel[0] = Common.DotNetToOracle(UnusualAllowanceID.ToString());
                listModel[2] = Month;
                var UnAllowCfgAmount = baseService.GetData<Cat_UnAllowCfgAmountEntity>(listModel, ConstantSql.hrm_cat_sp_get_Cat_UnAllowCfgAmount, UserLogin, ref status).OrderByDescending(m => m.FromDate).FirstOrDefault();
                if (UnAllowCfgAmount != null)
                {
                    return Json(UnAllowCfgAmount.Amount != null ? UnAllowCfgAmount.Amount : 0);
                }
                return Json(0);
            }
            else
            {
                return Json(0);
            }
        }

        #region CostActivity
        public ActionResult GetCostActivity([DataSourceRequest] DataSourceRequest request, Cat_CostActivitySearch model)
        {
            return GetListDataAndReturn<Cat_CostActivityModel, Cat_CostActivityEntity, Cat_CostActivitySearch>(request, model, ConstantSql.hrm_cat_sp_get_CostActivity);
        }

        public ActionResult ExportAllCostActivityList([DataSourceRequest] DataSourceRequest request, Cat_CostActivitySearch model)
        {
            return ExportAllAndReturn<Cat_CostActivityEntity, Cat_CostActivityModel, Cat_CostActivitySearch>(request, model, ConstantSql.hrm_cat_sp_get_CostActivity);
        }

        public ActionResult ExportCostActivitySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CostActivityEntity, Cat_CostActivityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CostActivityByIds);
        }

        public JsonResult GetMultiCostActivity(string text)
        {
            return GetDataForControl<Cat_CostActivityModel, Cat_CostActivityEntity>(text, ConstantSql.hrm_cat_sp_get_CostActivity_Multi);
        }
        #endregion

        #region Cat_ElementMapToAccount

        public ActionResult GetElementMaptoAccount([DataSourceRequest] DataSourceRequest request, Cat_ElementMaptoAccountModel model)
        {
            //return GetListDataAndReturn<Cat_ElementMaptoAccountEntity, Cat_ElementMaptoAccountModel, Cat_ElementMaptoAccountModelSearch>(request, model, ConstantSql.hrm_cat_sp_get_ElementMaptoAccount);
            string status = string.Empty;
            var actionService = new ActionService(UserLogin);
            var isDataTable = false;
            object obj = new Cat_ElementMaptoAccountModel();

            Cat_ElementMaptoAccountModelSearch listObj = new Cat_ElementMaptoAccountModelSearch();
            //listObj = model.Copy<Cat_ElementMaptoAccountModelSearch>();
            listObj.ElementCode = model.ElementCode;
            listObj.CostActivityID = model.CostActivityID;
            listObj.DateStart = model.DateStart;
            listObj.DateEnd = model.DateEnd;
            ListQueryModel lstModel = new ListQueryModel
            {
                PageIndex = 1,
                PageSize = int.MaxValue - 1,
                Filters = ExtractFilterAttributes(request),
                Sorts = ExtractSortAttributes(request),
                UserLogin = UserLogin,
                AdvanceFilters = ExtractAdvanceFilterAttributes(listObj)
            };

            var result = actionService.GetData<Cat_ElementMaptoAccountEntity>(lstModel, ConstantSql.hrm_cat_sp_get_ElementMaptoAccount, ref status).Translate<Cat_ElementMaptoAccountModel>();
            List<Cat_ElementMaptoAccountModel> listResult = new List<Cat_ElementMaptoAccountModel>();
            foreach (var i in result)
            {
                if (!listResult.Any(m => m.ElementCode == i.ElementCode && m.CostActivityID == i.CostActivityID && m.DateEnd == i.DateEnd && m.DateStart == i.DateStart && m.GLAccount == i.GLAccount && m.DebitAccount == i.DebitAccount && m.InternalOrderNo == i.InternalOrderNo))
                {
                    listResult.Add(i);
                }
            }

            //result = result.Where(s => s.DateTransferPayment != null && Model.DateFrom != null && s.DateTransferPayment.Value.Month == Model.DateFrom.Value.Month && s.DateTransferPayment.Value.Year == Model.DateFrom.Value.Year && (s.IsPaymentOut == false || s.IsPaymentOut == null)).ToList();

            HeaderInfo headerInfo1 = new HeaderInfo() { Name = "DateStart", Value = model != null && model.DateStart != null ? model.DateStart.Value : DateTime.MinValue };
            HeaderInfo headerInfo2 = new HeaderInfo() { Name = "DateEnd", Value = model != null && model.DateEnd != null ? model.DateEnd.Value : DateTime.MinValue };
            List<HeaderInfo> listHeaderInfo = new List<HeaderInfo>() { headerInfo1, headerInfo2 };

            if (model != null && model.IsCreateTemplateForDynamicGrid)
            {
                obj = listResult;
                isDataTable = true;
            }
            if (model != null && model.IsCreateTemplate)
            {
                var path = Common.GetPath("Templates");
                ExportService exportService = new ExportService();

                ConfigExport cfgExport = new ConfigExport()
                {
                    Object = new Cat_ElementMaptoAccountModel(),
                    FileName = "Cat_ElementMaptoAccount",
                    OutPutPath = path,
                    HeaderInfo = listHeaderInfo,
                    DownloadPath = Hrm_Main_Web + "Templates",
                    IsDataTable = false
                };
                var str = exportService.CreateTemplate(cfgExport);
                return Json(str);
            }
            if (model.ExportID != Guid.Empty)
            {
                var fullPath = ExportService.Export(model.ExportID, listResult, listHeaderInfo, UserGuidID, ExportFileType.CSV);
                return Json(fullPath);
            }

            return Json(listResult.ToDataSourceResult(request));


        }

        public ActionResult ExportAllElementMaptoAccountList([DataSourceRequest] DataSourceRequest request, Cat_ElementMaptoAccountModelSearch model)
        {
            return ExportAllAndReturn<Cat_ElementMaptoAccountEntity, Cat_ElementMaptoAccountModel, Cat_ElementMaptoAccountModelSearch>(request, model, ConstantSql.hrm_cat_sp_get_ElementMaptoAccount);
        }

        public ActionResult ExportElementMaptoAccountSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ElementMaptoAccountEntity, Cat_ElementMaptoAccountModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ElementMaptoAccountByIds);
        }

        public ActionResult SaveChangeElementMaptoAccount([Bind(Prefix = "models")] List<Cat_ElementMaptoAccountModel> model)
        {
            Cat_ElementMaptoAccountServices Services = new Cat_ElementMaptoAccountServices();
            Services.SaveChangeElementMaptoAccount(model.Translate<Cat_ElementMaptoAccountEntity>());
            return Json("");
        }

        #endregion

        [HttpPost]
        public ActionResult GetSalaryClassByID(Guid? SalaryClassID)
        {
            if (SalaryClassID != Guid.Empty && SalaryClassID != null)
            {
                string status = string.Empty;
                var actionService = new ActionService(UserLogin);
                var salaryClassEntity = actionService.GetData<Cat_SalaryClassEntity>(Common.DotNetToOracle(SalaryClassID.ToString()),
                    ConstantSql.hrm_cat_sp_get_SalaryClassById, ref status).FirstOrDefault();
                return Json(salaryClassEntity, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        #region [Hien.Nguyen] lấy dữ liệu phòng ban theo ordernumber
        public ActionResult GetOrgStructureByOrder(string listOrdernumber, Guid userID, string selectedIDs)
        {
            ActionService actionserveice = new ActionService(UserLogin);
            List<Object> listObject = new List<object>();
            string status = string.Empty;
            listObject.Add(null);
            listObject.Add(null);
            var listEntity = baseService.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_cat_sp_get_OrgStructure_Data, UserLogin, ref status);

            string[] arrListOrderNumber = listOrdernumber.Split(',');
            string[] arrSelectedID = selectedIDs.Split(',');
            Guid ID = Guid.Empty;
            Guid.TryParse(Common.DotNetToOracle(arrSelectedID[0]), out ID);
            var dataPermission = actionserveice.GetByIdUseStore<Sys_DataPermissionEntity>(ID, ConstantSql.hrm_sys_sp_get_DataPermissionById, ref status);

            listEntity = listEntity.Where(m => arrListOrderNumber.Contains(m.OrderNumber.ToString())).ToList();
            return Json(new List<object> { listEntity.Select(m => m.Code + " - " + m.OrgStructureName), listEntity.Select(m => m.ID), dataPermission }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public JsonResult GetMultiRewardedDecidingOrgs(string text)
        {
            return GetDataForControl<Cat_RewardedDecidingOrgsMultiModel, Cat_RewardedDecidingOrgsMultiEntity>(text, ConstantSql.Hrm_Cat_Sp_Get_RewardedDecidingOrgs_Multi);
        }

        [HttpPost]
        public JsonResult GetMultiRewardedTitles(string text)
        {
            return GetDataForControl<Cat_RewardedTitlesMultiModel, Cat_RewardedTitlesMultiEntity>(text, ConstantSql.Hrm_Cat_Sp_Get_RewardedTitles_Multi);
        }

        [HttpPost]
        public JsonResult GetMultiRewardedTime(string text)
        {
            return GetDataForControl<Cat_RewardedTimeMultiModel, Cat_RewardedTimeMultiEntity>(text, ConstantSql.Hrm_Cat_Sp_Get_RewardedTime_Multi);
        }

        #region Cat_RewardedTime
        [HttpPost]
        public ActionResult GetRewardedTimeList([DataSourceRequest] DataSourceRequest request, Cat_RewardedTimeSearchModel model)
        {
            return GetListDataAndReturn<Cat_RewardedTimeEntity, Cat_RewardedTimeModel, Cat_RewardedTimeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedTime);
        }
        public ActionResult ExportAllGetRewardedTimeList([DataSourceRequest] DataSourceRequest request, Cat_RewardedTimeSearchModel model)
        {
            return ExportAllAndReturn<Cat_RewardedTimeEntity, Cat_RewardedTimeModel, Cat_RewardedTimeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedTime);
        }
        public ActionResult ExportGetRewardedTimeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RewardedTimeModel, Cat_RewardedTimeEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RewardedTimeByIds);
        }
        #endregion

        #region Cat_RewardedTitles
        [HttpPost]
        public ActionResult GetRewardedTitlesList([DataSourceRequest] DataSourceRequest request, Cat_RewardedTitlesSearchModel model)
        {
            return GetListDataAndReturn<Cat_RewardedTitlesEntity, Cat_RewardedTitlesModel, Cat_RewardedTitlesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedTitles);
        }
        public ActionResult ExportAllGetRewardedTitlesList([DataSourceRequest] DataSourceRequest request, Cat_RewardedTitlesSearchModel model)
        {
            return ExportAllAndReturn<Cat_RewardedTitlesEntity, Cat_RewardedTitlesModel, Cat_RewardedTitlesSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedTitles);
        }
        public ActionResult ExportGetRewardedTitlesSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RewardedTitlesModel, Cat_RewardedTitlesEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RewardedTitlesByIds);
        }
        #endregion

        #region Cat_RewardedDecidingOrgs
        [HttpPost]
        public ActionResult GetRewardedDecidingOrgsList([DataSourceRequest] DataSourceRequest request, Cat_RewardedDecidingOrgsSearchModel model)
        {
            return GetListDataAndReturn<Cat_RewardedDecidingOrgsEntity, Cat_RewardedDecidingOrgsModel, Cat_RewardedDecidingOrgsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedDecidingOrgs);
        }
        public ActionResult ExportAllGetRewardedDecidingOrgsList([DataSourceRequest] DataSourceRequest request, Cat_RewardedDecidingOrgsSearchModel model)
        {
            return ExportAllAndReturn<Cat_RewardedDecidingOrgsEntity, Cat_RewardedDecidingOrgsModel, Cat_RewardedDecidingOrgsSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RewardedDecidingOrgs);
        }
        public ActionResult ExportGetRewardedDecidingOrgsSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RewardedDecidingOrgsModel, Cat_RewardedDecidingOrgsEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RewardedDecidingOrgsByIds);
        }
        #endregion
        public JsonResult GetAmountByChildCareAndFromDate(Guid UnusualEDTypeID, DateTime FromDate)
        {
            Cat_UnAllowCfgAmountServices reportService = new Cat_UnAllowCfgAmountServices();
            var result = reportService.GetAmountByChildCareAndFromDate(UnusualEDTypeID, FromDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Cat_UsualAllowanceGroup
        [HttpPost]
        public ActionResult GetUsualAllowanceGroupList([DataSourceRequest] DataSourceRequest request, Cat_UsualAllowanceGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_UsualAllowanceGroupEntity, Cat_UsualAllowanceGroupModel, Cat_UsualAllowanceGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UsualAllowanceGroup);
        }
        public ActionResult ExportAllGetUsualAllowanceGroupList([DataSourceRequest] DataSourceRequest request, Cat_UsualAllowanceGroupSearchModel model)
        {
            return ExportAllAndReturn<Cat_UsualAllowanceGroupEntity, Cat_UsualAllowanceGroupModel, Cat_UsualAllowanceGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UsualAllowanceGroup);
        }
        public ActionResult ExportGetUsualAllowanceGroupSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_UsualAllowanceGroupModel, Cat_UsualAllowanceGroupEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_UsualAllowanceGroupByIds);
        }
        #endregion

        #region Cat_Major
        [HttpPost]
        public ActionResult GetMajorList([DataSourceRequest] DataSourceRequest request, Cat_MajorSearchModel model)
        {
            return GetListDataAndReturn<Cat_MajorEntity, Cat_MajorModel, Cat_MajorSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Major);
        }
        public ActionResult ExportAllGetMajorList([DataSourceRequest] DataSourceRequest request, Cat_MajorSearchModel model)
        {
            return ExportAllAndReturn<Cat_MajorEntity, Cat_MajorModel, Cat_MajorSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Major);
        }
        public ActionResult ExportGetMajorSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_MajorModel, Cat_MajorEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_MajorByIds);
        }
        #endregion

        #region Cat_SubMajor
        [HttpPost]
        public ActionResult GetSubMajorList([DataSourceRequest] DataSourceRequest request, Cat_SubMajorSearchModel model)
        {
            return GetListDataAndReturn<Cat_SubMajorEntity, Cat_SubMajorModel, Cat_SubMajorSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SubMajor);
        }
        public ActionResult ExportAllGetSubMajorList([DataSourceRequest] DataSourceRequest request, Cat_SubMajorSearchModel model)
        {
            return ExportAllAndReturn<Cat_SubMajorEntity, Cat_SubMajorModel, Cat_SubMajorSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SubMajor);
        }
        public ActionResult ExportGetSubMajorSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SubMajorModel, Cat_SubMajorEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SubMajorByIds);
        }
        #endregion

        #region Cat_GetMulti
        public JsonResult GetMultiSubMajor(string text)
        {
            return GetDataForControl<CatSubMajorMultiModel, CatSubMajorMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SubMajor_multi);
        }
        public JsonResult GetMultiMajor(string text)
        {
            return GetDataForControl<CatMajorMultiModel, CatMajorMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Major_multi);
        }
        public JsonResult GetMultiPriceType(string text)
        {
            return GetDataForControl<Cat_PriceTypeMultiModel, Cat_PriceTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_PriceType_multi);
        }
        #endregion

        #region [12/01/2016][Phuc.Nguyen][New Func][0062976] Thêm màn hình Cat_PriceType
        [HttpPost]
        public ActionResult GetPriceTypeList([DataSourceRequest] DataSourceRequest request, Cat_PriceTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_PriceTypeEntity, Cat_PriceTypeModel, Cat_PriceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PriceType);
        }
        public ActionResult ExportAllGetPriceTypeList([DataSourceRequest] DataSourceRequest request, Cat_PriceTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_PriceTypeEntity, Cat_PriceTypeModel, Cat_PriceTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PriceType);
        }
        public ActionResult ExportGetPriceTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PriceTypeModel, Cat_PriceTypeEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PriceTypeByIds);
        }
        #endregion

        #region [12/01/2016][Phuc.Nguyen][New Func][0062976] Thêm màn hình Cat_PriceTypeDetail
        [HttpPost]
        public ActionResult GetPriceTypeDetailList([DataSourceRequest] DataSourceRequest request, Cat_PriceTypeDetailSearchModel model)
        {
            return GetListDataAndReturn<Cat_PriceTypeDetailEntity, Cat_PriceTypeDetailModel, Cat_PriceTypeDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PriceTypeDetail);
        }
        public ActionResult ExportAllGetPriceTypeDetailList([DataSourceRequest] DataSourceRequest request, Cat_PriceTypeDetailSearchModel model)
        {
            return ExportAllAndReturn<Cat_PriceTypeDetailEntity, Cat_PriceTypeDetailModel, Cat_PriceTypeDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PriceTypeDetail);
        }
        public ActionResult ExportGetPriceTypeDetailSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PriceTypeDetailModel, Cat_PriceTypeDetailEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PriceTypeDetailByIds);
        }
        #endregion

        #region [27/10/2015][Phuc.Nguyen][New Func][0058993] Thêm màn hình Cat_ElementReportHeadcount
        [HttpPost]
        public ActionResult GetElementReportHeadcountList([DataSourceRequest] DataSourceRequest request, Cat_ElementReportHeadcountSearchModel model)
        {
            return GetListDataAndReturn<Cat_ElementReportHeadcountEntity, Cat_ElementReportHeadcountModel, Cat_ElementReportHeadcountSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ElementReportHead);
        }
        public ActionResult ExportAllGetElementReportHeadcountList([DataSourceRequest] DataSourceRequest request, Cat_ElementReportHeadcountSearchModel model)
        {
            return ExportAllAndReturn<Cat_ElementReportHeadcountEntity, Cat_ElementReportHeadcountModel, Cat_ElementReportHeadcountSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ElementReportHead);
        }
        public ActionResult ExportGetElementReportHeadcountSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ElementReportHeadcountModel, Cat_ElementReportHeadcountEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ElementReportHeadByIds);
        }
        #endregion

        #region [15/12/2015][Phuc.Nguyen][New Func][0061761] Thêm màn hình Cat_RegionDetail
        [HttpPost]
        public ActionResult GetRegionDetailList([DataSourceRequest] DataSourceRequest request, Cat_RegionDetailSearchModel model)
        {
            return GetListDataAndReturn<Cat_RegionDetailEntity, Cat_RegionDetailModel, Cat_RegionDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RegionDetail);
        }
        public ActionResult ExportAllGetRegionDetailList([DataSourceRequest] DataSourceRequest request, Cat_RegionDetailSearchModel model)
        {
            return ExportAllAndReturn<Cat_RegionDetailEntity, Cat_RegionDetailModel, Cat_RegionDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_RegionDetail);
        }
        public ActionResult ExportGetRegionDetailSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_RegionDetailModel, Cat_RegionDetailEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_RegionDetailByIds);
        }
        #endregion

        #region [29/12/2015][Phuc.Nguyen][New Func][0062458] Thêm màn hình Cat_CompanyRate
        [HttpPost]
        public ActionResult GetCompanyRateList([DataSourceRequest] DataSourceRequest request, Cat_CompanyRateSearchModel model)
        {
            return GetListDataAndReturn<Cat_CompanyRateEntity, Cat_CompanyRateModel, Cat_CompanyRateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CompanyRate);
        }
        public ActionResult ExportAllGetCompanyRateList([DataSourceRequest] DataSourceRequest request, Cat_CompanyRateSearchModel model)
        {
            return ExportAllAndReturn<Cat_CompanyRateEntity, Cat_CompanyRateModel, Cat_CompanyRateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CompanyRate);
        }
        public ActionResult ExportGetCompanyRateSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CompanyRateModel, Cat_CompanyRateEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CompanyRateByIds);
        }
        #endregion

        #region [03/02/2016][Phuc.Nguyen][Assign Task][0063640] Thêm màn hình Cat_SalarySurvey
        [HttpPost]
        public ActionResult GetSalarySurveyList([DataSourceRequest] DataSourceRequest request, Cat_SalarySurveySearchModel model)
        {
            return GetListDataAndReturn<Cat_SalarySurveyEntity, Cat_SalarySurveyModel, Cat_SalarySurveySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalarySurvey);
        }
        public ActionResult ExportAllGetSalarySurveyList([DataSourceRequest] DataSourceRequest request, Cat_SalarySurveySearchModel model)
        {
            return ExportAllAndReturn<Cat_SalarySurveyEntity, Cat_SalarySurveyModel, Cat_SalarySurveySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalarySurvey);
        }
        public ActionResult ExportGetSalarySurveySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SalarySurveyModel, Cat_SalarySurveyEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalarySurveyByIds);
        }
        #endregion

        #region [16/02/2016][Phuc.Nguyen][Assign Task][0063641] Thêm màn hình Cat_ComputingSkill
        [HttpPost]
        public ActionResult GetComputingSkillList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        #endregion

        #region [28/03/2016][Phuc.Nguyen][New Func][0065480] Thêm màn hình Cat_ArchivesType
        [HttpPost]
        public ActionResult GetArchivesTypeList([DataSourceRequest] DataSourceRequest request, Cat_LevelSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_LevelSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelGeneral);
        }
        #endregion

        #region [19/02/2016][Phuc.Nguyen][New Func][0063815] Thêm màn hình Cat_GradeSalDept
        public JsonResult GetMultiSalarySurvey(string text)
        {
            return GetDataForControl<Cat_SalarySurveyMultiModel, Cat_SalarySurveyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SalarySurvey_multi);
        }
        [HttpPost]
        public ActionResult GetGradeSalDeptList([DataSourceRequest] DataSourceRequest request, Cat_GradeSalDeptSearchModel model)
        {
            return GetListDataAndReturn<Cat_GradeSalDeptEntity, Cat_GradeSalDeptModel, Cat_GradeSalDeptSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GradeSalDept);
        }
        public ActionResult ExportAllGetGradeSalDeptList([DataSourceRequest] DataSourceRequest request, Cat_GradeSalDeptSearchModel model)
        {
            return ExportAllAndReturn<Cat_GradeSalDeptEntity, Cat_GradeSalDeptModel, Cat_GradeSalDeptSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GradeSalDept);
        }
        public ActionResult ExportGetGradeSalDeptSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_GradeSalDeptModel, Cat_GradeSalDeptEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_GradeSalDeptByIds);
        }
        #endregion

        #region [05/03/2016][Phuc.Nguyen][New Func][0064214] Thêm màn hình Cat_Dormitory
        public JsonResult GetMultiDormitory(string text)
        {
            return GetDataForControl<Cat_DormitoryMultiModel, Cat_DormitoryMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Dormitory_multi);
        }
        [HttpPost]
        public ActionResult GetDormitoryList([DataSourceRequest] DataSourceRequest request, Cat_DormitorySearchModel model)
        {
            return GetListDataAndReturn<Cat_DormitoryEntity, Cat_DormitoryModel, Cat_DormitorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Dormitory);
        }
        public ActionResult ExportAllGetDormitoryList([DataSourceRequest] DataSourceRequest request, Cat_DormitorySearchModel model)
        {
            return ExportAllAndReturn<Cat_DormitoryEntity, Cat_DormitoryModel, Cat_DormitorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Dormitory);
        }
        public ActionResult ExportGetDormitorySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_DormitoryModel, Cat_DormitoryEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_DormitoryByIds);
        }
        #endregion

        #region [12/03/2016][Phuc.Nguyen][New Func][0064726] Thêm màn hình Cat_GradeSalDeptElement
        public JsonResult GetGradeSalDeptElementMulti(string text)
        {
            return GetDataForControl<Cat_GradeSalDeptElementMultiModel, Cat_GradeSalDeptElementMultiEntity>(text, ConstantSql.hrm_cat_sp_get_GradeSalDeptElement_multi);
        }
        [HttpPost]
        public ActionResult GetGradeSalDeptElementList([DataSourceRequest] DataSourceRequest request, Cat_GradeSalDeptElementSearchModel model)
        {
            return GetListDataAndReturn<Cat_GradeSalDeptElementEntity, Cat_GradeSalDeptElementModel, Cat_GradeSalDeptElementSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GradeSalDeptElement);
        }
        public ActionResult ExportAllGetGradeSalDeptElementList([DataSourceRequest] DataSourceRequest request, Cat_GradeSalDeptElementSearchModel model)
        {
            return ExportAllAndReturn<Cat_GradeSalDeptElementEntity, Cat_GradeSalDeptElementModel, Cat_GradeSalDeptElementSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GradeSalDeptElement);
        }
        public ActionResult ExportGetGradeSalDeptElementSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_GradeSalDeptElementModel, Cat_GradeSalDeptElementEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_GradeSalDeptElementByIds);
        }
        #endregion

        #region [05/03/2016][Phuc.Nguyen][New Func][0064214] Thêm màn hình Cat_Folder
        public JsonResult GetMultiFolder(string text)
        {
            return GetDataForControl<Cat_FolderMultiModel, Cat_FolderMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Folder_multi);
        }
        [HttpPost]
        public ActionResult GetFolderList([DataSourceRequest] DataSourceRequest request, Cat_FolderSearchModel model)
        {
            return GetListDataAndReturn<Cat_FolderEntity, Cat_FolderModel, Cat_FolderSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Folder);
        }
        public ActionResult ExportAllGetFolderList([DataSourceRequest] DataSourceRequest request, Cat_FolderSearchModel model)
        {
            return ExportAllAndReturn<Cat_FolderEntity, Cat_FolderModel, Cat_FolderSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Folder);
        }
        public ActionResult ExportGetFolderSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_FolderModel, Cat_FolderEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_FolderByIds);
        }
        #endregion

        public ActionResult GetReqTraDocumentByPositionID([DataSourceRequest] DataSourceRequest request, Guid? positionID)
        {
            if (positionID != null)
            {
                string status = string.Empty;
                var actionServices = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(positionID);
                List<Cat_ReqTraDocumentEntity> result = new List<Cat_ReqTraDocumentEntity>();
                var data = actionServices.GetData<Cat_ReqTraDocumentEntity>(Common.DotNetToOracle(positionID.ToString()), ConstantSql.hrm_hr_sp_get_ReqTraDocumentByPositionId, ref status);
                if (data != null && data.Count > 0)
                {
                    result = data;
                }
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }

        public JsonResult GetMultiSkill(string text)
        {
            return GetDataForControl<Cat_SkillMultiModel, Cat_SkillMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Skill_Multi);
        }

        public ActionResult GetPositionSkillByPositionID([DataSourceRequest] DataSourceRequest request, Guid? positionID)
        {
            if (positionID != null)
            {
                string status = string.Empty;
                var actionServices = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(positionID);
                List<Cat_PositionSkillEntity> result = new List<Cat_PositionSkillEntity>();
                var data = actionServices.GetData<Cat_PositionSkillEntity>(Common.DotNetToOracle(positionID.ToString()), ConstantSql.hrm_hr_sp_get_PositionSkillByPositionId, ref status);
                if (data != null && data.Count > 0)
                {
                    result = data;
                }
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }

        [HttpPost]
        public ActionResult SetStatusOrg(string orgNumber, string status)
        {
            var service = new Cat_OrgStructureServices();
            var message = service.SetStatusOrgStructure(orgNumber, status);
            return Json(message);
        }

        [HttpPost]
        public ActionResult GetDataOfContractType(string contractTypeID)
        {
            if (!string.IsNullOrEmpty(contractTypeID))
            {
                Guid id = Guid.Parse(contractTypeID);
                var contractTypeServices = new Cat_ContractTypeServices();
                var entity = contractTypeServices.GetDataContractTypeByID(id);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        //Son.Vo - 20161101 - 0074744
        [HttpPost]
        public ActionResult GetDateByContractTypeID(int? duration, DateTime? dateStart, string unitTime)
        {
            if (duration != null && dateStart != null && unitTime != null)
            {
                var contracttype = new Cat_ContractTypeEntity();
                if (unitTime == EnumDropDown.UnitType.E_DAY.ToString())
                {
                    contracttype.DateEnd = dateStart.Value.AddDays(duration.Value);
                }
                else if (unitTime == EnumDropDown.UnitType.E_MONTH.ToString())
                {
                    contracttype.DateEnd = dateStart.Value.AddMonths(duration.Value);
                }
                else if (unitTime == EnumDropDown.UnitType.E_YEAR.ToString())
                {
                    contracttype.DateEnd = dateStart.Value.AddYears(duration.Value);
                }
                if (contracttype.DateEnd != null)
                {
                    contracttype.DateEnd = contracttype.DateEnd.Value.AddDays(-1);
                }
                return Json(contracttype, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetDurationByDate(DateTime? dateStart, DateTime? dateEnd)
        {
            if (dateStart != null && dateEnd != null)
            {
                var contracttype = new Cat_ContractTypeEntity();
                contracttype.ValueTime = Math.Round((double)(dateEnd.Value.Subtract(dateStart.Value).Days / (365.25 / 12)));
                return Json(contracttype, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        // Son.Vo - 20151117 - 0060242
        public ActionResult IsConfigApprovedContract()
        {
            var actionService = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            var lstConfigStatus = new List<object>();
            lstConfigStatus.Add(null);
            lstConfigStatus.Add(1);
            lstConfigStatus.Add(int.MaxValue - 1);
            var configStatus = actionService.GetData<Sys_ConfigDefaultStatusEntity>(lstConfigStatus, ConstantSql.sys_sp_get_ConfigDefaultStatus, ref status).Where(s => s.TableName == "Hre_Contract").FirstOrDefault();
            if (configStatus != null && (configStatus.FieldName == "Status" && configStatus.DefaultValue == "E_APPROVED"))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetdefautValue(string tableName, string columnName, string fieldType)
        {
            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnName))
            {
                var actionService = new ActionService(UserLogin, LanguageCode);
                string status = string.Empty;
                var lstConfigStatus = new List<object>();
                lstConfigStatus.Add(tableName);
                lstConfigStatus.Add(columnName);
                lstConfigStatus.Add(fieldType);
                var configStatus = actionService.GetData<DefaultValueMultiModel>(lstConfigStatus, ConstantSql.sys_sp_get_ConfigDefaultValue, ref status);
                return Json(configStatus, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null);
            }
        }

        #region Cat_Branch
        [HttpPost]
        public ActionResult GetBranchList([DataSourceRequest] DataSourceRequest request, Cat_BranchSearchModel model)
        {
            return GetListDataAndReturn<Cat_BranchModel, Cat_BranchEntity, Cat_BranchSearchModel>(request, model, ConstantSql.cat_sp_get_Branch);
        }

        public ActionResult ExportAllBranchList([DataSourceRequest] DataSourceRequest request, Cat_BranchSearchModel model)
        {
            return ExportAllAndReturn<Cat_BranchEntity, Cat_BranchModel, Cat_BranchSearchModel>(request, model, ConstantSql.cat_sp_get_Branch);
        }


        public JsonResult GetMultiBranch(string text)
        {
            string status = string.Empty;
            BaseService baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(text);
            objs.Add(text);
            objs.Add(null);
            objs.Add(null);
            objs.Add(1);
            objs.Add(int.MaxValue - 1);
            var result = baseService.GetData<Cat_BranchEntity>(objs, ConstantSql.cat_sp_get_Branch, UserLogin, ref status).OrderBy(s => s.BranchName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBanchInfoByBankMulti(string text, Guid? BankID)
        {
            BaseService baseService = new BaseService();
            string status = string.Empty;
            var para_Branch = new List<object>();
            para_Branch.AddRange(new object[5]);
            if (!string.IsNullOrEmpty(text))
            {
                para_Branch[0] = text;
                para_Branch[1] = text;
            }
            if (BankID != null && BankID != Guid.Empty)
            {
                para_Branch[2] = BankID;
            }
            para_Branch[3] = 1;
            para_Branch[4] = int.MaxValue - 1;
            var result = baseService.GetData<Cat_BranchEntity>(para_Branch, ConstantSql.cat_sp_get_BranchByBankID, UserLogin, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBanchInfoByBank(Guid? BankID)
        {
            if (BankID != null)
            {
                BaseService baseService = new BaseService();
                string status = string.Empty;
                var objs = new List<object>();
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(null);
                objs.Add(1);
                objs.Add(int.MaxValue - 1);
                var result = baseService.GetData<Cat_BranchEntity>(objs, ConstantSql.cat_sp_get_Branch, UserLogin, ref status).Where(s => s.BankID == BankID).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBanchByID(Guid? BranchID)
        {
            if (BranchID != null)
            {
                BaseService baseService = new BaseService();
                string status = string.Empty;
                var result = baseService.GetData<Cat_BranchEntity>(Common.DotNetToOracle(BranchID.ToString()), ConstantSql.cat_sp_get_BranchById, UserLogin, ref status).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Cat_Project
        public ActionResult GetProjectList([DataSourceRequest] DataSourceRequest request, Cat_ProjectSearchModel model)
        {
            return GetListDataAndReturn<Cat_ProjectModel, Cat_ProjectEntity, Cat_ProjectSearchModel>(request, model, ConstantSql.cat_sp_get_Project);
        }
        public ActionResult ExportProjectSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ProjectEntity, Cat_ProjectModel>(selectedIds, valueFields, ConstantSql.cat_sp_get_ProjectByIds);
        }
        public ActionResult ExportAllProjectList([DataSourceRequest] DataSourceRequest request, Cat_ProjectSearchModel model)
        {
            return ExportAllAndReturn<Cat_ProjectEntity, Cat_ProjectModel, Cat_ProjectSearchModel>(request, model, ConstantSql.cat_sp_get_Project);
        }
        #endregion

        #region Cat_ImportOT
        /// <summary>
        /// [Chuc.Nguyen] - Lấy danh sách dữ liệu(Cat_ImportOT)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetImportOTList([DataSourceRequest] DataSourceRequest request, Cat_ImportOTSearchModel model)
        {
            return GetListDataAndReturn<Cat_ImportOTModel, Cat_ImportOTEntity, Cat_ImportOTSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ImportOT);
        }

        public ActionResult ExportAllImportOTList([DataSourceRequest] DataSourceRequest request, Cat_ImportOTSearchModel model)
        {
            return ExportAllAndReturn<Cat_ImportOTEntity, Cat_ImportOTModel, Cat_ImportOTSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ImportOT);
        }

        public JsonResult GetMultiImportOT(string text)
        {
            return GetDataForControl<Cat_ImportOTMultiModel, Cat_ImportOTMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ImportOT_Multi);
        }
        #endregion

        #region Cat_Company
        [HttpPost]
        public ActionResult GetCompanyList([DataSourceRequest] DataSourceRequest request, Cat_CompanySearchModel model)
        {
            return GetListDataAndReturn<Cat_CompanyModel, Cat_CompanyEntity, Cat_CompanySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Company);
        }
        public JsonResult GetMultiCompany(string text)
        {
            return GetDataForControl<Cat_CompanyMultiModel, Cat_CompanyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Company_multi);
        }

        public ActionResult ExportAllCompanyList([DataSourceRequest] DataSourceRequest request, Cat_CompanySearchModel model)
        {
            return ExportAllAndReturn<Cat_CompanyEntity, Cat_CompanyModel, Cat_CompanySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Company);
        }
        public ActionResult GetInfoForCompanyByProfileID(Guid ProfileID)
        {
            Hre_ProfileForCompanyEntity objProfile = new Hre_ProfileForCompanyEntity();
            var service = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            var Profile = service.GetData<Hre_ProfileForCompanyEntity>(Common.DotNetToOracle(ProfileID.ToString()), ConstantSql.hrm_cat_sp_get_ProfileBySelectProID, ref status).FirstOrDefault();
            if (Profile != null)
            {
                objProfile = Profile;
            }
            return Json(objProfile);
        }

        public ActionResult GetDelegateCompanyByCompanyID([DataSourceRequest] DataSourceRequest request, Guid? companyID)
        {
            if (companyID != null && companyID != Guid.Empty)
            {
                string status = string.Empty;
                var service = new ActionService(UserLogin, LanguageCode);
                var obj = new List<object>();
                obj.AddRange(new object[4]);
                obj[0] = companyID;
                obj[1] = null;
                obj[2] = 1;
                obj[3] = int.MaxValue - 1;
                var result = service.GetData<Cat_DelegateCompanyModel>(obj, ConstantSql.hrm_cat_sp_get_DelegateCompanybyCompanyID, ref status);
                if (result != null)
                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                return Json(null);
            }
            return Json(null);
        }
        #endregion

        #region Cat_EnumTranslate
        public ActionResult GetEnumTranslateList([DataSourceRequest] DataSourceRequest request, Cat_EnumTranslateSearchModel model)
        {
            return GetListDataAndReturn<Cat_EnumTranslateModel, Cat_EnumTranslateEntity, Cat_EnumTranslateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EnumTranslate);
        }

        public ActionResult ExportEnumTranslateSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_EnumTranslateEntity, Cat_EnumTranslateModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_EnumTranslatByIds);
        }

        public ActionResult ExportAllEnumTranslateList([DataSourceRequest] DataSourceRequest request, Cat_EnumTranslateSearchModel model)
        {
            return ExportAllAndReturn<Cat_EnumTranslateEntity, Cat_EnumTranslateModel, Cat_EnumTranslateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_EnumTranslate);
        }
        #endregion

        //[03122015][bang.nguyen][61231][new func]
        //Thêm Chức năng Cập nhật dữ liệu trên màn hình Ds cấu hình dịch enum
        public ActionResult UpdateCat_EnumTranslate()
        {
            Sys_GeneralServices generalServices = new Sys_GeneralServices();
            generalServices.UpdateCat_EnumTranslate();
            return Json("Success");
        }

        public JsonResult GetEthnicGroup_webix()
        {
            string value = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                value = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var service = new BaseService();
            var result = new List<Cat_DataForControllModel>();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_EthnicGroupMultiEntity>(value, ConstantSql.hrm_cat_sp_get_EthnicGroup_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.EthnicGroupName }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosition_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_PositionMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Position_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.PositionName }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountry_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_CountryMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Country_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.CountryName }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReligion_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_ReligionMultiModel>(text, ConstantSql.hrm_cat_sp_get_religion_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.ReligionName }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSourceAds_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_SourceAdsMultiEntity>(text, ConstantSql.hrm_cat_sp_get_SourceAds_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.SourceAdsName }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrictCascading_webix()
        {
            string text = string.Empty;
            var arrayText = new string[2];
            arrayText[0] = string.Empty;
            arrayText[1] = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                for (int i = 0; i < Request.QueryString.AllKeys.Length; i++)
                {
                    arrayText[i] = Request.QueryString[Request.QueryString.AllKeys[i].ToString()].ToString() != Guid.Empty.ToString() ? Request.QueryString[Request.QueryString.AllKeys[i].ToString()].ToString() : string.Empty;
                }
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<CatDistrictModel>(arrayText[0], ConstantSql.hrm_cat_sp_get_DisctrictByProvinceId, UserLogin, ref status);
            if (listEntity != null)
            {
                if (!string.IsNullOrEmpty(arrayText[1]))
                {
                    listEntity = listEntity.Where(s => s.DistrictName.ToUpper().IndexOf(arrayText[1].ToUpper()) != -1).ToList();
                }
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.DistrictName.ToUpper() }).ToList();
                result = result.OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProvinceCascading_webix()
        {

            string text = string.Empty;
            var arrayText = new string[2];
            arrayText[0] = string.Empty;
            arrayText[1] = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                for (int i = 0; i < Request.QueryString.AllKeys.Length; i++)
                {
                    arrayText[i] = Request.QueryString[Request.QueryString.AllKeys[i].ToString()].ToString() != Guid.Empty.ToString() ? Request.QueryString[Request.QueryString.AllKeys[i].ToString()].ToString() : string.Empty;
                }
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<CatProvinceModel>(arrayText[0], ConstantSql.hrm_cat_sp_get_ProvinceByCountryId, UserLogin, ref status);
            if (listEntity != null)
            {
                if (!string.IsNullOrEmpty(arrayText[1]))
                {
                    listEntity = listEntity.Where(s => s.ProvinceName.ToUpper().IndexOf(arrayText[1].ToUpper()) != -1).ToList();
                }
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.ProvinceName.ToUpper() }).ToList();
                result = result.OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWardCascading_webix()
        {
            string text = string.Empty;
            var arrayText = new string[2];
            arrayText[0] = string.Empty;
            arrayText[1] = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                for (int i = 0; i < Request.QueryString.AllKeys.Length; i++)
                {
                    arrayText[i] = Request.QueryString[Request.QueryString.AllKeys[i].ToString()].ToString() != Guid.Empty.ToString() ? Request.QueryString[Request.QueryString.AllKeys[i].ToString()].ToString() : string.Empty;
                }
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_VillageModel>(arrayText[0], ConstantSql.hrm_cat_sp_get_VillageByDistrictId, UserLogin, ref status);
            if (listEntity != null)
            {
                if (!string.IsNullOrEmpty(arrayText[1]))
                {
                    listEntity = listEntity.Where(s => s.VillageName.ToUpper().IndexOf(arrayText[1].ToUpper()) != -1).ToList();
                }
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.VillageName.ToUpper() }).ToList();
                result = result.OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRelativeType_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_RelativeTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_RelativeType_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.RelativeTypeName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComputingSpecialLevel_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_QualificationMultiLevelEntity>(text, ConstantSql.hrm_cat_sp_get_ComputingLevel_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.NameEntityName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgStructureMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_OrgStructureMultiEntity>(text, ConstantSql.hrm_cat_sp_get_OrgStructure_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.OrgStructureName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfileMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Hre_ProfileMultiEntity>(text, ConstantSql.hrm_hr_sp_get_AllProfile_multiV2, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Where(s => s.ProfileName != null && s.ID != Guid.Empty).Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.ProfileName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkPlaceMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_WorkPlaceMultiEntity>(text, ConstantSql.hrm_cat_sp_get_WorkPlace_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.WorkPlaceName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobVacancyMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Rec_JobVacancyMultiEntity>(text, ConstantSql.hrm_rec_sp_get_JobVacancy_multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.JobVacancyName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQualificationMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_QualificationMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Qualification_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.QualificationName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducationLevelMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_EducationLevel_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.NameEntityName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHospitalMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_HospitalEntity>(text, ConstantSql.hrm_cat_sp_get_Hospital_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.HospitalName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLanguageTypelMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_LanguageTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LanguageType_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.NameEntityName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLanguageLevelMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_LanguageTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LanguageLevel_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.NameEntityName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLanguageSkillMulti_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_LanguageTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_LanguageSkill_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.NameEntityName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComputingSpecialType_webix()
        {
            string text = string.Empty;
            if (Request.QueryString.AllKeys.Length > 0)
            {
                text = Request.QueryString[Request.QueryString.AllKeys[0].ToString()].ToString();
            }
            var result = new List<Cat_DataForControllModel>();
            var service = new BaseService();
            string status = string.Empty;
            var listEntity = service.GetData<Cat_ComputingTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_ComputingType_Multi, UserLogin, ref status);
            if (listEntity != null)
            {
                result = listEntity.Select(s => new Cat_DataForControllModel { id = s.ID.ToString(), value = s.NameEntityName }).OrderBy(s => s.value).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgStructureByOrderNumber(string oderNo)
        {
            string status = string.Empty;
            var actionserveice = new ActionService(UserLogin);
            var listObject = new List<object>();
            listObject.Add(oderNo);
            var listEntity = actionserveice.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_cat_sp_get_OrgStructureByOrderNumberV2, ref status);

            if (listEntity != null && listEntity.Count > 0)
            {
                return Json(new List<object> { listEntity.Select(m => m.Code + " - " + m.OrgStructureName), listEntity.Select(m => m.ID) }, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<object> { null, null }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrderNumberElement(string _gradePayrollIDs)
        {
            Cat_ElementServices _elementServices = new Cat_ElementServices();
            _elementServices.UpdateOrderNumberElement(_gradePayrollIDs);
            return Json(new ResultsObject());
        }

        #region Cat_MultiUsualAllowanceGroup
        [HttpPost]
        public JsonResult GetMultiUsualAllowanceGroup(string text)
        {
            return GetDataForControl<Cat_UsualAllowanceGroupMultiModel, Cat_UsualAllowanceGroupMultiEntity>(text, ConstantSql.Cat_Sp_Get_UsualAllowanceGroup_Multi);
        }
        #endregion

        public ActionResult GetAmountByAllowanceTypeID([DataSourceRequest] DataSourceRequest request, Guid? AllowanceTypeID)
        {
            if (AllowanceTypeID != null)
            {
                string status = string.Empty;
                var actionService = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(Common.DotNetToOracle(AllowanceTypeID.ToString()));
                var result = actionService.GetData<Cat_UsualAllowanceModel>(Common.DotNetToOracle(AllowanceTypeID.ToString()), ConstantSql.hrm_cat_sp_get_UsualAllowanceById, ref status);
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }

        #region Cat_PayrollCategory
        public ActionResult GetPayrollCategoryList([DataSourceRequest] DataSourceRequest request, Cat_PayrollCategorySearchModel model)
        {
            return GetListDataAndReturn<Cat_PayrollCategoryEntity, Cat_PayrollCategoryModel, Cat_PayrollCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PayrollCategory);
        }
        public ActionResult ExportAllGetPayrollCategoryList([DataSourceRequest] DataSourceRequest request, Cat_PayrollCategorySearchModel model)
        {
            return ExportAllAndReturn<Cat_PayrollCategoryEntity, Cat_PayrollCategoryModel, Cat_PayrollCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PayrollCategory);
        }
        public ActionResult ExportGetPayrollCategorySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_PayrollCategoryModel, Cat_PayrollCategoryEntity>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PayrollCategoryByIds);
        }
        #endregion

        public ActionResult GetAddressMulti(string text)
        {
            var result = new List<Cat_AddressModel>();
            var actionser = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            result = actionser.GetData<Cat_AddressModel>(text, ConstantSql.hrm_cat_sp_get_address_Multi, ref status);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataSoureAddress(Guid ID)
        {
            var result = new List<Cat_AddressDataSourceModel>();
            var actionser = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            result = actionser.GetData<Cat_AddressDataSourceModel>(Common.DotNetToOracle(ID.ToString()), ConstantSql.hrm_cat_sp_get_addressdatasrc, ref status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFolderPath(Guid FolderID)
        {
            Cat_FolderServices _ser = new Cat_FolderServices();
            var physicalPath = _ser.GetPathFolder(FolderID);
            physicalPath = string.Format("Archives\\{0}", physicalPath);
            physicalPath = physicalPath.Replace("\\", "/");
            return Json(physicalPath, JsonRequestBehavior.AllowGet);
        }

        #region Cat_CardType
        [HttpPost]
        public ActionResult GetCardTypeList([DataSourceRequest] DataSourceRequest request, Cat_CardTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_CardTypeModel, Cat_CardTypeEntity, Cat_CardTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CardType);
        }

        public ActionResult ExportAllCardTypeList([DataSourceRequest] DataSourceRequest request, Cat_CardTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_CardTypeEntity, Cat_CardTypeModel, Cat_CardTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CardType);
        }

        public ActionResult GetMultiCardType(string text)
        {
            return GetDataForControl<Cat_CardTypeModel, Cat_CardTypeEntity>(text, ConstantSql.hrm_cat_sp_get_CardType_Multi);
        }

        #endregion

        #region Cat_SpecialArea
        [HttpPost]
        public ActionResult GetSpecialAreaList([DataSourceRequest] DataSourceRequest request, Cat_SpecialAreaSearchModel model)
        {
            return GetListDataAndReturn<Cat_SpecialAreaModel, Cat_SpecialAreaEntity, Cat_SpecialAreaSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SpecialArea);
        }

        public ActionResult ExportAllSpecialAreaList([DataSourceRequest] DataSourceRequest request, Cat_SpecialAreaSearchModel model)
        {
            return ExportAllAndReturn<Cat_SpecialAreaEntity, Cat_SpecialAreaModel, Cat_SpecialAreaSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SpecialArea);
        }

        public ActionResult GetMultiSpecialArea(string text)
        {
            return GetDataForControl<Cat_SpecialAreaModel, Cat_SpecialAreaEntity>(text, ConstantSql.hrm_cat_sp_get_SpecialArea_Multi);
        }

        #endregion

        #region Test Grid Excel
        [HttpPost]
        public ActionResult GetProductTypeListTest([DataSourceRequest] DataSourceRequest request)
        {
            //CatProductTypeSearchModel model = new CatProductTypeSearchModel();
            //DataSourceRequest request = new DataSourceRequest();
            //return GetListDataAndReturn<CatProductTypeModel, Cat_ProductTypeEntity, CatProductTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ProductType);

            string status = string.Empty;
            //var service = new ActionService(UserLogin);
            List<object> _listmodel = new List<object>();
            _listmodel = Common.AddRange(4);
            var listEntity = baseService.GetData<Cat_BankTestEntity>(_listmodel, ConstantSql.hrm_cat_sp_get_Bank, UserLogin, ref status);
            return Json(listEntity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ProductType_Update([DataSourceRequest] DataSourceRequest request)
        {
            List<Cat_BankTestEntity> models = JsonConvert.DeserializeObject<List<Cat_BankTestEntity>>(Request.QueryString[0]);
            Cat_ProductTypeServices services = new Cat_ProductTypeServices();
            services.EditRange(models);
            return Json(models.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ProductType_Create([DataSourceRequest] DataSourceRequest request)
        {
            var _listModel = JsonConvert.DeserializeObject(Request.QueryString[0]);

            List<Cat_BankTestEntity> models = JsonConvert.DeserializeObject<List<Cat_BankTestEntity>>(Request.QueryString[0].Replace("\"ID\":\"\"", "\"ID\":\"" + Guid.Empty.ToString() + "\""));
            Cat_ProductTypeServices services = new Cat_ProductTypeServices();
            services.AddRange(models);
            return Json(models.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductType_Delete([DataSourceRequest] DataSourceRequest request)
        {
            List<Cat_ProductTypeEntity> models = JsonConvert.DeserializeObject<List<Cat_ProductTypeEntity>>(Request.QueryString[0]);
            Cat_ProductTypeServices services = new Cat_ProductTypeServices();
            services.DeleteRange(models.Select(m => m.ID).ToList());
            return Json(models.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Google Chart

        public JsonResult GetDataExampleAreaChart()
        {
            //List<List<object>> _listobject = new List<List<object>>();
            ArrayList _listobject = new ArrayList();
            List<asdasd> result = new List<asdasd>();

            for (int i = 1; i < 10; i++)
            {
                asdasd item = new asdasd();
                item.Year = (2013 + i).ToString();
                item.Value1 = 3434 + i;
                item.Value1 = 123 + i;
                result.Add(item);
            }



            //List<object> item = new List<object>();
            //item.Add("Year");
            //item.Add("Sales");
            //item.Add("Expenses");
            //_listobject.Add(item);

            //item = new List<object>();
            //item.Add("2013");
            //item.Add(1000);
            //item.Add(400);
            //_listobject.Add(item);

            //item = new List<object>();
            //item.Add("2014");
            //item.Add(1170);
            //item.Add(1170);
            //_listobject.Add(item);

            //item = new List<object>();
            //item.Add("2015");
            //item.Add(660);
            //item.Add(1120);
            //_listobject.Add(item);

            //item = new List<object>();
            //item.Add("2016");
            //item.Add(1030);
            //item.Add(540);
            //_listobject.Add(item);

            //string _json = "['Year', 'Sales', 'Expenses'],['2013', 1000, 400],['2014', 1170, 1170],['2015', 660, 1120],['2016', 1030, 540]";
            return Json(result.ToArrayList<asdasd>(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeavedayPieChart(DateTime? DateFrom, DateTime? DateTo)
        {
            DateFrom = new DateTime(2016, 6, 1);
            DateTo = new DateTime(2016, 6, 30);

            var random = new Random();
            var _Ser = new ActionService(UserLogin);
            string status = string.Empty;
            var lstObj = new List<object>();
            lstObj.AddRange(new object[2]);
            lstObj[0] = DateFrom;
            lstObj[1] = DateTo;
            var _data = _Ser.GetData<LeavedayPieChartModel>(lstObj, ConstantSql.hrm_sys_sp_get_LeavedayPieChart, ref status);
            List<Hre_DashBoardPieChartModel> items = new List<Hre_DashBoardPieChartModel>();
            if (!Common.CheckListNullOrEmty(_data))
            {
                foreach (var item in _data)
                {
                    items.Add(new Hre_DashBoardPieChartModel { category = item.LeaveDayTypeName, value = item.RateLeave, monthyear = item.monthyear, color = GetRandomColor(random.Next(1000)) });
                }
            }
            return Json(items.ToArrayList<Hre_DashBoardPieChartModel>(), JsonRequestBehavior.AllowGet);
        }
        private string GetRandomColor(int rd)
        {
            var random = new Random(rd);
            var color = String.Format("#{0:X6}", random.Next(0x1000000));
            return color;
        }
        public class asdasd
        {
            public string Year { get; set; }
            public int Value1 { get; set; }
            public int Value2 { get; set; }
        }

        #endregion

        #region Cat_WorkList
        [HttpPost]
        public ActionResult GetWorkListList([DataSourceRequest] DataSourceRequest request, Cat_WorkListSearchModel model)
        {
            return GetListDataAndReturn<Cat_WorkListModel, Cat_WorkListEntity, Cat_WorkListSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_WorkList);
        }

        public ActionResult ExportWorkListSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_WorkListEntity, Cat_WorkListModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_WorkListByIds);
        }

        public JsonResult GetMultiWorkList(string text)
        {
            return GetDataForControl<Cat_WorkListMultiModel, Cat_WorkListMultiEntity>(text, ConstantSql.hrm_cat_sp_get_WorkList_multi);
        }

        //Son.Vo - 20160817 - theo yêu cầu TienDang
        public JsonResult GetMultiWorkListManual(string text)
        {
            return GetDataForControl<Cat_WorkListMultiModel, Cat_WorkListMultiEntity>(text, ConstantSql.hrm_cat_sp_get_WorkListManual_multi);
        }

        public JsonResult GetMultiQuitWorkList(string text)
        {
            return GetDataForControl<Cat_WorkListMultiModel, Cat_WorkListMultiEntity>(text, ConstantSql.hrm_cat_sp_get_QuitWorkList_multi);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllWorkListList([DataSourceRequest] DataSourceRequest request, Cat_WorkListSearchModel model)
        {
            return ExportAllAndReturn<Cat_WorkListEntity, Cat_WorkListModel, Cat_WorkListSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_WorkList);
        }
        #endregion

        #region Cat_PositionWorkList
        public ActionResult GetPositionWorkListByPositionID([DataSourceRequest] DataSourceRequest request, Guid? positionID)
        {
            if (positionID != null)
            {
                string status = string.Empty;
                var actionServices = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(positionID);
                var result = new List<Cat_PositionWorkListEntity>();
                var data = actionServices.GetData<Cat_PositionWorkListEntity>(Common.DotNetToOracle(positionID.ToString()), ConstantSql.hrm_cat_sp_get_PositionWorkListByPositionId, ref status);
                if (data != null && data.Count > 0)
                {
                    result = data.Where(s => s.Type == EnumDropDown.WorkListType.E_NEW.ToString()).ToList();
                }
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }

        public ActionResult GetPositionQuitWorkListByPositionID([DataSourceRequest] DataSourceRequest request, Guid? positionID)
        {
            if (positionID != null)
            {
                string status = string.Empty;
                var actionServices = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(positionID);
                var result = new List<Cat_PositionWorkListEntity>();
                var data = actionServices.GetData<Cat_PositionWorkListEntity>(Common.DotNetToOracle(positionID.ToString()), ConstantSql.hrm_cat_sp_get_PositionWorkListByPositionId, ref status);
                if (data != null && data.Count > 0)
                {
                    result = data.Where(s => s.Type == EnumDropDown.WorkListType.E_QUIT.ToString()).ToList();
                }
                return Json(result.ToDataSourceResult(request));
            }
            return null;

        }

        [HttpPost]
        public ActionResult GetWorklistDataByID(Guid? WorklistID)
        {
            if (WorklistID != null)
            {
                string status = string.Empty;
                var service = new ActionService(UserLogin, LanguageCode);
                var entity = service.GetByIdUseStore<Cat_WorkListEntity>(WorklistID.Value, ConstantSql.hrm_cat_sp_get_WorkListById, ref status);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        [HttpPost]
        public ActionResult GetUnitStructureList([DataSourceRequest] DataSourceRequest request, Cat_UnitStructureSearchModel model)
        {
            return GetListDataAndReturn<Cat_UnitStructureModel, Cat_UnitStructureEntity, Cat_UnitStructureSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UnitStructure);
        }

        public ActionResult ExportAllUnitStructureList([DataSourceRequest] DataSourceRequest request, Cat_UnitStructureSearchModel model)
        {
            return ExportAllAndReturn<Cat_UnitStructureEntity, Cat_UnitStructureModel, Cat_UnitStructureSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_UnitStructure);
        }

        public ActionResult GetMultiUnitStructure(string text)
        {
            return GetDataForControl<Cat_UnitStructureMultiModel, Cat_UnitStructureMultiEntity>(text, ConstantSql.hrm_cat_sp_get_UnitStructure_Multi);
        }

        #region Cat_HealthTreatmentPlace
        [HttpPost]
        public ActionResult GetHealthTreatmentPlaceList([DataSourceRequest] DataSourceRequest request, Cat_HealthTreatmentPlaceSearchModel model)
        {
            return GetListDataAndReturn<Cat_HealthTreatmentPlaceModel, Cat_HealthTreatmentPlaceEntity, Cat_HealthTreatmentPlaceSearchModel>(request, model, ConstantSql.hrm_Cat_sp_get_HealthTreatmentPlace);
        }

        public ActionResult ExportHealthTreatmentPlaceSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_HealthTreatmentPlaceEntity, Cat_HealthTreatmentPlaceModel>(selectedIds, valueFields, ConstantSql.hrm_Cat_sp_get_HealthTreatmentPlaceByIds);
        }

        public ActionResult ExportAlltHealthTreatmentPlaceList([DataSourceRequest] DataSourceRequest request, Cat_HealthTreatmentPlaceSearchModel model)
        {
            return ExportAllAndReturn<Cat_HealthTreatmentPlaceEntity, Cat_HealthTreatmentPlaceModel, Cat_HealthTreatmentPlaceSearchModel>(request, model, ConstantSql.hrm_Cat_sp_get_HealthTreatmentPlace);
        }

        public JsonResult GetMultiHealthTreatmentPlace(string text)
        {
            return GetDataForControl<Cat_HealthTreatmentPlaceModel, Cat_HealthTreatmentPlaceEntity>(text, ConstantSql.hrm_cat_sp_get_HealthTreatmentPlace_Multi);
        }

        public ActionResult GetHealthTreatmentPlaceById(Guid ID)
        {
            var actionser = new ActionService(UserLogin, LanguageCode);
            string status = string.Empty;
            var _WorkPlaceServices = new Cat_WorkPlaceServices();
            var result = _WorkPlaceServices.getHealthTreatmentPlaceByID(ID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetServeryQuestion([DataSourceRequest] DataSourceRequest request, Guid? SurveyID)
        {
            if (SurveyID != null)
            {
                string status = string.Empty;
                var actionService = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(Common.DotNetToOracle(SurveyID.ToString()));
                var result = actionService.GetData<Cat_SurveyQuestionModel>(Common.DotNetToOracle(SurveyID.ToString()), ConstantSql.hrm_cat_sp_get_SurveyQuestionById, ref status);
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }

        public ActionResult GetSurveyQuestionTypeByID([DataSourceRequest] DataSourceRequest request, Guid? SurveyQuestionTypeID)
        {
            if (SurveyQuestionTypeID != null)
            {
                string status = string.Empty;
                var actionService = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(Common.DotNetToOracle(SurveyQuestionTypeID.ToString()));
                var result = actionService.GetData<Cat_SurveyQuestionTypeModel>(Common.DotNetToOracle(SurveyQuestionTypeID.ToString()), ConstantSql.hrm_cat_sp_get_SurveyQuestionTypeById, ref status);
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }

        #region Cat_Organization
        [HttpPost]
        public ActionResult GetOrganizationList([DataSourceRequest] DataSourceRequest request, Cat_OrganizationSearchModel model)
        {
            return GetListDataAndReturn<Cat_OrganizationModel, Cat_OrganizationEntity, Cat_OrganizationSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Organization);
        }

        public ActionResult ExportOrganizationSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_OrganizationEntity, Cat_OrganizationModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_OrganizationByIds);
        }

        public JsonResult GetMultiOrganization(string text)
        {
            return GetDataForControl<Cat_OrganizationMultiModel, Cat_OrganizationMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Organization_multi);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllOrganizationList([DataSourceRequest] DataSourceRequest request, Cat_OrganizationSearchModel model)
        {
            return ExportAllAndReturn<Cat_OrganizationEntity, Cat_OrganizationModel, Cat_OrganizationSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Organization);
        }
        #endregion

        public JsonResult GetMultiSeniority(string text)
        {
            return GetDataForControl<Cat_ElementReportHeadcountMultiModel, Cat_ElementReportHeadcountMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Seniority_multi);
        }
        public JsonResult GetMultiAge(string text)
        {
            return GetDataForControl<Cat_ElementReportHeadcountMultiModel, Cat_ElementReportHeadcountMultiEntity>(text, ConstantSql.hrm_cat_sp_get_Age_multi);
        }

        #region FacilityIssuesCategory

        [HttpPost]
        public ActionResult GetFacilityIssuesCategoryList([DataSourceRequest] DataSourceRequest request, Cat_FacilityIssuesCategorySearchModel model)
        {
            return GetListDataAndReturn<Cat_FacilityIssuesCategoryModel, Cat_FacilityIssuesCategoryEntity, Cat_FacilityIssuesCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_FacilityIssuesCategory);
        }

        public ActionResult ExportAllFacilityIssuesCategoryList([DataSourceRequest] DataSourceRequest request, Cat_FacilityIssuesCategorySearchModel model)
        {
            return ExportAllAndReturn<Cat_FacilityIssuesCategoryEntity, Cat_FacilityIssuesCategoryModel, Cat_FacilityIssuesCategorySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_FacilityIssuesCategory);
        }

        public ActionResult ExportFacilityIssuesCategorySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_FacilityIssuesCategoryEntity, Cat_FacilityIssuesCategoryModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_FacilityIssuesCategoryByIds);
        }
        #endregion

        #region FacilityIssuesTemplate
        [HttpPost]
        public ActionResult GetFacilityIssueTemplateList([DataSourceRequest] DataSourceRequest request, Cat_FacilityIssueTemplateSearchModel model)
        {
            return GetListDataAndReturn<Cat_FacilityIssueTemplateModel, Cat_FacilityIssueTemplateEntity, Cat_FacilityIssueTemplateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_FacilityIssueTemplate);
        }

        public ActionResult ExportFacilityIssueTemplateSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_FacilityIssueTemplateModel, Cat_FacilityIssueTemplateModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_FacilityIssueTemplateByIds);
        }

        [HttpPost]
        public ActionResult ExportAllFacilityIssueTemplateList([DataSourceRequest] DataSourceRequest request, Cat_FacilityIssueTemplateSearchModel model)
        {
            return ExportAllAndReturn<Cat_FacilityIssueTemplateModel, Cat_FacilityIssueTemplateModel, Cat_FacilityIssueTemplateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_FacilityIssueTemplate);
        }

        public ActionResult GetFacilityIssueTemplateItemByTempalteIDList([DataSourceRequest] DataSourceRequest request, Guid TemplateID)
        {
            string status = string.Empty;
            var baseService = new BaseService();
            var objs = new List<object>();
            objs.Add(TemplateID);
            var result = baseService.GetData<Cat_FacilityTemplateItemModel>(objs, ConstantSql.hrm_cat_sp_get_FacTemplateItemByTemplateID, UserLogin, ref status);
            return Json(result.ToDataSourceResult(request));
        }

        #endregion

        #region Cat_SalaryRankDetail
        [HttpPost]
        public ActionResult GetSalaryRankDetailList([DataSourceRequest] DataSourceRequest request, Cat_SalaryRankDetailSearchModel model)
        {
            return GetListDataAndReturn<Cat_SalaryRankDetailModel, Cat_SalaryRankDetailEntity, Cat_SalaryRankDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryRankDetail);
        }

        public ActionResult ExportSalaryRankDetailSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_SalaryRankDetailEntity, Cat_SalaryRankDetailModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_SalaryRankDetailByIds);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllSalaryRankDetailList([DataSourceRequest] DataSourceRequest request, Cat_SalaryRankDetailSearchModel model)
        {
            return ExportAllAndReturn<Cat_SalaryRankDetailEntity, Cat_SalaryRankDetailModel, Cat_SalaryRankDetailSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SalaryRankDetail);
        }
        #endregion

        //Son.Vo - 20161017 - kiểm tra file trên lưới có tồn tại hay không khi click vào link để dowload file.
        public ActionResult CheckFileExist(string TemplateFile)
        {
            if (!string.IsNullOrEmpty(TemplateFile))
            {
                string templatepath = Common.GetPath(Common.TemplateURL + TemplateFile);
                if (!System.IO.File.Exists(templatepath))
                {
                    return Json("NotTemplate", JsonRequestBehavior.AllowGet);
                }
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMultiCostRecruitment(string text)
        {
            return GetDataForControl<Cat_NameEntityMultiModel, Cat_NameEntityMultiEntity>(text, ConstantSql.hrm_cat_sp_get_CostRecruitment_Multi);
        }

        #region chuc vu >> tab tieu chuan chi phi/thiet bi
        /// <summary>Tiêu Chuẩn Chi Phí/Thiết Bị</summary>
        /// <param name="request"></param>
        /// <param name="positionID"></param>
        /// <returns></returns>
        public ActionResult GetStandardCostByPositionID([DataSourceRequest] DataSourceRequest request, Guid? positionID)
        {
            if (positionID != null)
            {
                string status = string.Empty;
                var actionServices = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(positionID);
                List<Cat_PositionOfFacilityModel> result = new List<Cat_PositionOfFacilityModel>();
                var data = actionServices.GetData<Cat_PositionOfFacilityModel>(Common.DotNetToOracle(positionID.ToString()), ConstantSql.hrm_hr_sp_get_PositionOfFacilityByPositionId, ref status);
                //var data = new List<Cat_PositionOfFacilityModel>();
                if (data != null && data.Count > 0)
                {
                    result = data;
                }
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }
        #endregion

        public JsonResult GetEnumGenderFilter()
        {
            var ActionService = new ActionService(UserLogin, LanguageCode);
            var enumType = Utilities.GetEnumType("Gender",
             typeof(EnumDropDown.Gender).Assembly);
            var service = new EnumService();
            var listGender = service.GetEnumDataInfo(enumType);
            listGender.ForEach(s => s.Translate = s.Name.TranslateString(LanguageCode));
            return Json(listGender);
        }

        public JsonResult GetEnumMarriageStatusFilter()
        {
            var ActionService = new ActionService(UserLogin, LanguageCode);
            var enumType = Utilities.GetEnumType("MarriageStatus",
             typeof(EnumDropDown.MarriageStatus).Assembly);
            var service = new EnumService();
            var listGender = service.GetEnumDataInfo(enumType);
            listGender.ForEach(s => s.Translate = s.Name.TranslateString(LanguageCode));
            return Json(listGender);
        }

        public JsonResult GetEnumEqualCondition()
        {
            var ActionService = new ActionService(UserLogin, LanguageCode);
            var enumType = Utilities.GetEnumType("ValueType",
             typeof(EnumDropDown.MarriageStatus).Assembly);
            var service = new EnumService();
            var listenumType = service.GetEnumDataInfo(enumType);
            listenumType = listenumType.Where(s => s.Translate == HRM.Infrastructure.Utilities.ValueType.E_EQUAL.ToString().Translate()).ToList();
            listenumType.ForEach(s => s.Translate = s.Name.TranslateString(LanguageCode));
            return Json(listenumType);
        }

        public JsonResult GetEnumMasterDataCondition()
        {
            var ActionService = new ActionService(UserLogin, LanguageCode);
            var enumType = Utilities.GetEnumType("ValueType",
             typeof(EnumDropDown.MarriageStatus).Assembly);
            var service = new EnumService();
            var listenumType = service.GetEnumDataInfo(enumType);
            var lstData = new List<string>();
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_EQUAL.ToString().Translate());
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_VALUETYPEIN.ToString().Translate());
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_VALUETYPEALL.ToString().Translate());
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_NOT_EQUAL.ToString().Translate());
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_VALUETYPELIKE.ToString().Translate());
            listenumType = listenumType.Where(s => lstData.Contains(s.Translate)).ToList();
            listenumType.ForEach(s => s.Translate = s.Name.TranslateString(LanguageCode));
            return Json(listenumType);
        }

        public JsonResult GetEnumNumberCondition()
        {
            var ActionService = new ActionService(UserLogin, LanguageCode);
            var enumType = Utilities.GetEnumType("ValueType",
             typeof(EnumDropDown.MarriageStatus).Assembly);
            var service = new EnumService();
            var listenumType = service.GetEnumDataInfo(enumType);
            var lstData = new List<string>();
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_VALUETYPEIN.ToString().Translate());
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_VALUETYPEALL.ToString().Translate());
            lstData.Add(HRM.Infrastructure.Utilities.ValueType.E_VALUETYPELIKE.ToString().Translate());
            listenumType = listenumType.Where(s => !lstData.Contains(s.Translate)).ToList();
            listenumType.ForEach(s => s.Translate = s.Name.TranslateString(LanguageCode));
            return Json(listenumType);
        }

        [HttpPost]
        public ActionResult GetSurveyQuestionGroupList([DataSourceRequest] DataSourceRequest request, Cat_SurveyQuestionGroupSearchModel model)
        {
            return GetListDataAndReturn<Cat_SurveyQuestionGroupModel, Cat_SurveyQuestionGroupEntity, Cat_SurveyQuestionGroupSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_SurveyQuestionGroup);
        }

        public JsonResult GetMultiQuestionGroup(string text)
        {
            return GetDataForControl<Cat_SurveyQuestionGroupModel, Cat_SurveyQuestionGroupMultiEntity>(text, ConstantSql.hrm_cat_sp_get_QuestionGroup_Multi);
        }


        #region Cat_MethodTraining
        public ActionResult GetMethodTrainingList([DataSourceRequest] DataSourceRequest request, Cat_MethodTrainingSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_MethodTrainingSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_MethodTraining);
        }
        public ActionResult ExportMethodTrainingSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_MethodTrainingByIds);
        }
        public ActionResult ExportAllMethodTrainingList([DataSourceRequest] DataSourceRequest request, Cat_MethodTrainingSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_MethodTrainingSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_MethodTraining);
        }
        public JsonResult GetMultiMethodTraining(string text)
        {
            return GetDataForControl<CatNameEntityTraMultiModel, Cat_NameEntityTraMultiEntity>(text, ConstantSql.hrm_cat_sp_get_MethodTraining_Multi);
        }
        #endregion

        #region Cat_PurposeTraining
        public ActionResult GetPurposeTrainingList([DataSourceRequest] DataSourceRequest request, Cat_PurposeTrainingSearchModel model)
        {
            return GetListDataAndReturn<CatNameEntityModel, Cat_NameEntityEntity, Cat_PurposeTrainingSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PurposeTraining);
        }
        public ActionResult ExportPurposeTrainingSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_NameEntityEntity, CatNameEntityModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_PurposeTrainingByIds);
        }
        public ActionResult ExportAllPurposeTrainingList([DataSourceRequest] DataSourceRequest request, Cat_PurposeTrainingSearchModel model)
        {
            return ExportAllAndReturn<Cat_NameEntityEntity, CatNameEntityModel, Cat_PurposeTrainingSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_PurposeTraining);
        }
        public JsonResult GetMultiPurposeTraining(string text)
        {
            return GetDataForControl<CatNameEntityTraMultiModel, Cat_NameEntityTraMultiEntity>(text, ConstantSql.hrm_cat_sp_get_PurposeTraining_Multi);
        }
        #endregion

        #region Cat_Meal
        public ActionResult GetMealList([DataSourceRequest] DataSourceRequest request, Cat_MealSearchModel model)
        {
            return GetListDataAndReturn<Cat_MealEntity, Cat_MealModel, Cat_MealSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Meal);
        }
        public JsonResult GetMealMulti(string text)
        {
            return GetDataForControl<Cat_MealMultiModel, Cat_MealMultiEntity>(text, ConstantSql.hrm_cat_sp_get_MealMulti);
        }

        public ActionResult ExportAllMealList([DataSourceRequest] DataSourceRequest request, Cat_MealSearchModel model)
        {
            return ExportAllAndReturn<Cat_MealEntity, Cat_MealModel, Cat_MealSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Meal);
        }

        public ActionResult ExportMealSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_MealEntity, Cat_MealModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_MealByIds);
        }

        #endregion

        public JsonResult GetMultiEvaluationFormula(string text, string EnumType)
        {
            var listNameEntityMultiEntity = new List<Cat_NameEntityMultiEntity>();
            Cat_NameEntityServices nameEntityServices = new Cat_NameEntityServices();
            listNameEntityMultiEntity = nameEntityServices.GetMultiEvaluationFormula(text, EnumType);
            return new JsonResult() { Data = listNameEntityMultiEntity, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetMultiEvaluationFormulaByActpercent(string text)
        {
            string EnumType = "E_PER_FORM_ACTPERCENT";
            var listNameEntityMultiEntity = new List<Cat_NameEntityMultiEntity>();
            Cat_NameEntityServices nameEntityServices = new Cat_NameEntityServices();
            listNameEntityMultiEntity = nameEntityServices.GetMultiEvaluationFormula(text, EnumType);
            return new JsonResult() { Data = listNameEntityMultiEntity, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetMultiEvaluationFormulaByScore(string text)
        {
            string EnumType = "E_PER_FORM_SCORE";
            var listNameEntityMultiEntity = new List<Cat_NameEntityMultiEntity>();
            Cat_NameEntityServices nameEntityServices = new Cat_NameEntityServices();
            listNameEntityMultiEntity = nameEntityServices.GetMultiEvaluationFormula(text, EnumType);
            return new JsonResult() { Data = listNameEntityMultiEntity, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetMultiEvaluationFormulaByTemplate(string text)
        {
            string EnumType = "E_EVA_TEMPLATE_SCORE";
            var listNameEntityMultiEntity = new List<Cat_NameEntityMultiEntity>();
            Cat_NameEntityServices nameEntityServices = new Cat_NameEntityServices();
            listNameEntityMultiEntity = nameEntityServices.GetMultiEvaluationFormula(text, EnumType);
            return new JsonResult() { Data = listNameEntityMultiEntity, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetMultiDataSource(string text)
        {
            return GetDataForControl<Sys_DataSourceModel, Sys_DataSourceEntity>(text, ConstantSql.hrm_sys_sp_get_DataSource_multi);
        }

        public ActionResult GetUsualAllowanceForPriotity()
        {
            Cat_UsualAllowanceServices usualAllowanceServices = new Cat_UsualAllowanceServices();
            var listResult = usualAllowanceServices.GetUsualAllowanceForPriotity();

            var listCat_UsualAllowance = new List<Cat_UsualAllowanceEntity>().Select(s => new { s.Priotity, s.ID, s.UsualAllowanceName }).ToList();
            for (int i = 0; i <= 15; i++)
            {
                var objUsualAllowanceByPriotity = listResult.Where(s => s.Priotity == i).Select(s => new { s.Priotity, s.ID, s.UsualAllowanceName }).FirstOrDefault();
                if (objUsualAllowanceByPriotity != null)
                {
                    listCat_UsualAllowance.Add(objUsualAllowanceByPriotity);
                }
                else
                {
                    listCat_UsualAllowance.Add(null);
                }
            }
            return Json(listCat_UsualAllowance);
        }

        public ActionResult GetCat_PolicyCompanyList([DataSourceRequest] DataSourceRequest request, Cat_PolicyCompanySearchModel model)
        {
            return GetListDataAndReturn<Cat_PolicyCompanyModel, Cat_PolicyCompanyEntity, Cat_PolicyCompanySearchModel>(request, model, ConstantSql.hrm_sys_sp_get_CompanyPolicy);
        }

        public JsonResult strConDefaultVa_TAMScanReasonMiss()
        {
            var _service = new ActionService(UserLogin);
            string status = string.Empty;
            var para = new List<object>();
            para.AddRange(new object[2]);
            para[0] = "Cat_TAMScanReasonMiss";
            para[1] = null;

            var result = _service.GetData<Sys_ConfigDefaultStatusEntity>(para, ConstantSql.sys_sp_get_ConfigDefaultStatus_Default, ref status);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Cat_GroupLevelCompetency
        [HttpPost]
        public ActionResult GetGroupLevelCompetencyList([DataSourceRequest] DataSourceRequest request, Cat_GroupLevelCompetencySearchModel model)
        {
            return GetListDataAndReturn<Cat_GroupLevelCompetencyModel, Cat_GroupLevelCompetencyEntity, Cat_GroupLevelCompetencySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GroupLevelCompetency);
        }
        public ActionResult ExportGroupLevelCompetencySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_GroupLevelCompetencyEntity, Cat_GroupLevelCompetencyModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_GroupLevelCompetencyByIds);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllGroupLevelCompetencyList([DataSourceRequest] DataSourceRequest request, Cat_GroupLevelCompetencySearchModel model)
        {
            return ExportAllAndReturn<Cat_GroupLevelCompetencyEntity, Cat_GroupLevelCompetencyModel, Cat_GroupLevelCompetencySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_GroupLevelCompetency);
        }
        public JsonResult GetMultiGroupLevelCompetency(string text)
        {
            return GetDataForControl<Cat_GroupLevelCompetencyMultiModel, Cat_GroupLevelCompetencyMultiEntity>(text, ConstantSql.hrm_cat_sp_get_GroupLevelCompetency_Multi);
        }
        #endregion

        #region Cat_LevelCompetency
        [HttpPost]
        public ActionResult GetLevelCompetencyList([DataSourceRequest] DataSourceRequest request, Cat_LevelCompetencySearchModel model)
        {
            return GetListDataAndReturn<Cat_LevelCompetencyModel, Cat_LevelCompetencyEntity, Cat_LevelCompetencySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelCompetency);
        }

        public ActionResult ExportLevelCompetencySelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_LevelCompetencyEntity, Cat_LevelCompetencyModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_LevelCompetencyByIds);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllLevelCompetencyList([DataSourceRequest] DataSourceRequest request, Cat_LevelCompetencySearchModel model)
        {
            return ExportAllAndReturn<Cat_LevelCompetencyEntity, Cat_LevelCompetencyModel, Cat_LevelCompetencySearchModel>(request, model, ConstantSql.hrm_cat_sp_get_LevelCompetency);
        }

        public JsonResult GetMultiLevelCompetency(string text)
        {
            return GetDataForControl<Cat_LevelCompetencyModel, Cat_LevelCompetencyEntity>(text, ConstantSql.hrm_cat_sp_get_LevelCompetency_Multi);
        }

        public JsonResult GetMultiLevelCompetencyBySKill(string text, Guid? SkillID)
        {
            var lstLevelCompetency = new List<Cat_LevelCompetencyMultiEntity>();
            ActionService services = new ActionService(UserLogin);
            var catSkillServices = new Cat_SkillServices();
            if (SkillID != null)
            {
                var LevelCompetency = catSkillServices.GetSkillByID(SkillID.Value);
                if (LevelCompetency != null && LevelCompetency.GroupLevelCompetencyID != null)
                {
                    var obj = new List<object>();
                    string status = string.Empty;
                    obj.Add(text);
                    obj.Add(LevelCompetency.GroupLevelCompetencyID);
                    obj.Add(1);
                    obj.Add(int.MaxValue - 1);
                    lstLevelCompetency = services.GetData<Cat_LevelCompetencyMultiEntity>(obj, ConstantSql.hrm_cat_sp_get_LevelCompetency_MultiBySkill, ref status);
                    return Json(lstLevelCompetency, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(lstLevelCompetency, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCourseTypeList([DataSourceRequest] DataSourceRequest request, Cat_CourseTypeSearchModel model)
        {
            return GetListDataAndReturn<Cat_CourseTypeModel, Cat_CourseTypeEntity, Cat_CourseTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CourseType);
        }

        public ActionResult ExportCourseTypeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_CourseTypeEntity, Cat_CourseTypeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_CourseTypeByIds);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllCourseTypeList([DataSourceRequest] DataSourceRequest request, Cat_CourseTypeSearchModel model)
        {
            return ExportAllAndReturn<Cat_CourseTypeEntity, Cat_CourseTypeModel, Cat_CourseTypeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_CourseType);
        }

        public JsonResult GetMultiCompetenceGroupNotParent(string text, Guid? ID)
        {
            var services = new ActionService(LanguageCode);
            string status = string.Empty;
            var result = services.GetData<Cat_CompetenceGroupEntity>(text, ConstantSql.hrm_cat_sp_get_CompetenceGroup_Multi, ref status);
            if (ID != null)
            {
                result = result.Where(s => s.ID != ID).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLevelCompetencyByGroup([DataSourceRequest] DataSourceRequest request, Guid? groupID)
        {
            if (groupID != null)
            {
                string status = string.Empty;
                var actionService = new ActionService(UserLogin);
                var objs = new List<object>();
                objs.Add(groupID);
                var result = actionService.GetData<Cat_LevelCompetencyEntity>(objs, ConstantSql.hrm_cat_sp_get_LevelCompetencyByGroupLevelId, ref status);
                return Json(result.ToDataSourceResult(request));
            }
            return null;
        }
        #endregion
        public JsonResult GetCompetenceGroup(Guid? id, string UserName)
        {
            var service = new Cat_OrgStructureServices();
            string status = string.Empty;
            var listModel = new List<Cat_CompetenceGroupModel>();
            if (HttpContext.Cache["CompetenceGroup_" + UserName] == null)
            {
                var objCompetenGroup = Common.AddRange(4);
                listModel = service.GetData<Cat_CompetenceGroupModel>(objCompetenGroup, ConstantSql.hrm_cat_sp_get_CompetenceGroup, UserLogin, ref status);
            }
            else
            {
                listModel = HttpContext.Cache["CompetenceGroup_" + UserName] as List<Cat_CompetenceGroupModel>;
            }




            //lấy quyền phòng ban theo user
            var orgStructure = from e in listModel
                               where (id.HasValue ? e.ParentID == id : e.ParentID == null)
                               select new
                               {
                                   id = e.ID,
                                   Name = e.Code + " - " + e.CompetenceGroupName,
                                   NameOrder = e.CompetenceGroupName,
                                   hasChildren = true,
                                   IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + "icon1.png",
                                   OrderNumber = 0,
                                   Code = e.Code,
                                   IsShow = false,
                                   OrderOrg = 0,
                                   Inactive = true,
                               };

            if (orgStructure.Count() == 0)
            {
                var objSkill = Common.AddRange(4);
                var listSkill = service.GetData<Cat_SkillEntity>(objSkill, ConstantSql.hrm_cat_sp_get_Skill, UserLogin, ref status);

                //lấy quyền phòng ban theo user
                orgStructure = from e in listSkill
                               where (e.CompetenceGroupID != null && e.CompetenceGroupID == id)
                               select new
                               {
                                   id = e.ID,
                                   Name = e.Code + " - " + e.SkillName,
                                   NameOrder = e.SkillName,
                                   hasChildren = false,
                                   IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + "icon1.png",
                                   OrderNumber = 0,
                                   Code = e.Code,
                                   IsShow = true,
                                   OrderOrg = 0,
                                   Inactive = false
                               };
                return Json(orgStructure, JsonRequestBehavior.AllowGet);
            }

            return Json(orgStructure, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompetenceGroupForCreate(Guid? id, string UserName)
        {
            var service = new Cat_OrgStructureServices();
            string status = string.Empty;
            var listModel = new List<Cat_CompetenceGroupModel>();
            if (HttpContext.Cache["CompetenceGroup_" + UserName] == null)
            {
                var objCompetenGroup = Common.AddRange(4);
                listModel = service.GetData<Cat_CompetenceGroupModel>(objCompetenGroup, ConstantSql.hrm_cat_sp_get_CompetenceGroup, UserLogin, ref status);
            }
            else
            {
                listModel = HttpContext.Cache["CompetenceGroup_" + UserName] as List<Cat_CompetenceGroupModel>;
            }

            var orgStructure = from e in listModel
                               where (id.HasValue ? e.ParentID == id : e.ParentID == null)
                               select new
                               {
                                   id = e.ID,
                                   Name = e.Code + " - " + e.CompetenceGroupName,
                                   NameOrder = listModel.Any(ch => ch.ParentID == e.ID),
                                   hasChildren = true,
                                   IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + "icon1.png",
                                   OrderNumber = 0,
                                   Code = e.Code,
                                   IsShow = !listModel.Any(ch => ch.ParentID == e.ID) ? true : false,
                                   OrderOrg = 0,
                                   Inactive = !listModel.Any(ch => ch.ParentID == e.ID) ? false : true,
                               };

            return Json(orgStructure, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiCourseType(string text)
        {
            return GetDataForControl<Cat_CourseTypeMultiModel, Cat_CourseTypeMultiEntity>(text, ConstantSql.hrm_cat_sp_get_CourseType_Multi);
        }
        //Toan.Vo lấy dữ liệu khóa học cho cây
        public JsonResult GetMultiCourseTypeTreeview(Guid? id, string text)
        {
            var service = new Cat_OrgStructureServices();
            string status = string.Empty;

            List<Object> listObject = new List<object>();
            listObject.Add(text == null ? "" : text);
            listObject.Add(1);
            listObject.Add(int.MaxValue - 1);
            var listModel = service.GetData<Cat_CourseTypeMultiModel>(listObject, ConstantSql.hrm_cat_sp_get_CourseType_Treeview, UserLogin, ref status);

            var orgStructure = from e in listModel
                               where (id.HasValue ? e.ParentID == id : e.ParentID == null)
                               select new
                               {
                                   id = e.ID,
                                   Name = e.CourseTypeName,
                                   hasChildren = listModel.Any(ch => ch.ParentID == e.ID),
                                   IconPath = ConstantPathWeb.HrWebUrl + ConstantPath.IconPath + ("icon1.png"),
                                   Code = e.Code,
                                   IsShow = true,
                                   Inactive = false,
                                   IsDisable = true
                               };
            var ListOrgCode = orgStructure.OrderBy(m => m.Name).ToList();
            return Json(ListOrgCode, JsonRequestBehavior.AllowGet);
        }
        //Toan.Vo lấy dữ liệu cây phòng ban theo bảng Cat_OrgHistory và Cat_OrgHistory.DecisionDate <= ngày hiệu lực (input param) có sum nhân viên theo phòng ban
        public JsonResult GetOrgTreeByHistorySumProfile(Guid? id, string text, DateTime? DateEffect)
        {
            var service = new Cat_OrgStructureServices();
            string status = string.Empty;
            var listModel = new List<CatOrgStructureModel>();
            if (DateEffect == null)
            {
                DateEffect = DateTime.Now;
            }
            //Quyen.Quach 24/08/2017 sửa tên cache phòng ban by history
            if (HttpContext.Cache["List_OrgStructureTreeViewbyHistorySumProfile_" + UserLogin] == null)
            {
                List<Object> listObject = new List<object>();
                listObject.Add(DateEffect);
                listObject.Add(UserLogin != null ? UserLogin : "");
                var listEntity = service.GetData<Cat_OrgStructureTreeViewEntity>(listObject, ConstantSql.hrm_hr_sp_get_OrgStructureHistory_Data_SumProfile, UserLogin, ref status);
                if (listEntity != null)
                {
                    listEntity = listEntity.GroupBy(p => p.OrgStructureID.Value).Select(p => p.Where(s => s.DecisionDate <= DateEffect).OrderByDescending(s => s.DecisionDate).FirstOrDefault()).Where(s => s != null).ToList();
                }

                if (listEntity != null)
                {
                    listModel = listEntity.Translate<CatOrgStructureModel>();
                    var tree = ConvertToOrgStructureTreeView(listModel, true);
                    var treeResult = GenerateOrgStructure(tree);
                    HttpContext.Cache.Add("List_OrgStructureTreeViewbyHistorySumProfile_" + UserLogin, treeResult, null, DateTime.Now.AddDays(30), TimeSpan.Zero, CacheItemPriority.Default, null);
                }
            }
            return Json(HttpContext.Cache["List_OrgStructureTreeViewbyHistorySumProfile_" + UserLogin], JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMultiCatLeaveDayType(string text)
        {
            return GetDataForControl<CatLeaveDayTypeModel, CatLeaveDayTypeModel>(text, ConstantSql.hrm_cat_sp_get_LeaveDayType_multi);
        }
        [HttpPost]
        public ActionResult GetCat_InsuranceGradeList([DataSourceRequest] DataSourceRequest request, Cat_InsuranceGradeSearchModel model)
        {
            return GetListDataAndReturn<Cat_InsuranceGradeModel, Cat_InsuranceGradeEntity, Cat_InsuranceGradeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_InsuranceGrade);
        }
        public ActionResult GetCat_InsuranceGradeDetailedList([DataSourceRequest] DataSourceRequest request, Cat_InsuranceGradeDetailedSearchModel model)
        {
            return GetListDataAndReturn<Cat_InsuranceGradeDetailedModel, Cat_InsuranceGradeDetailedEntity, Cat_InsuranceGradeDetailedSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_Cat_InsuranceGradeDetailed);
        }
        public JsonResult GetMultiInsuranceGrade(string text)
        {
            return GetDataForControl<Cat_InsuranceGradeModel, Cat_InsuranceGradeEntity>(text, ConstantSql.hrm_cat_sp_get_Multi_InsuranceGrade);
        }
        //Quyen.Quach 27/09/2017 0087411
        public ActionResult GetContractTemplateList([DataSourceRequest] DataSourceRequest request, Cat_ContractTemplateSearchModel model)
        {
            return GetListDataAndReturn<Cat_ContractTemplateModel, Cat_ContractTemplateEntity, Cat_ContractTemplateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ContractTemplate);
        }

        //Nguyen.Le - 20170928 - 00887411
        public string CheckDuplicateContractTemplate(Guid ID, Guid? contractTypeID, Guid? positionID, Guid? employeeTypeID, Guid? companyID)
        {
            var services = new Cat_ContractTemplateServices();
            string message = services.CheckDuplicateContractTemplate(ID, contractTypeID, positionID, employeeTypeID, companyID);
            return message;
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ExportAllContractTemplateList([DataSourceRequest] DataSourceRequest request, Cat_ContractTemplateSearchModel model)
        {
            return ExportAllAndReturn<Cat_ContractTemplateEntity, Cat_ContractTemplateModel, Cat_ContractTemplateSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ContractTemplate);
        }
        public ActionResult ExportContractTemplateSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ContractTemplateEntity, Cat_ContractTemplateModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ContractTemplateByIds);
        }

        ///test trước
        ///
        public ActionResult GetPerformanceEvaActiveList([DataSourceRequest] DataSourceRequest request, Hre_PerformanceEvaActiveSearchModel model)
        {
            return GetListDataAndReturn<Hre_PerformanceEvaActiveModel, Hre_PerformanceEvaActiveEntity, Hre_PerformanceEvaActiveSearchModel>(request, model, ConstantSql.hrm_hre_sp_get_PerformanceEvaActive);
        }
        public ActionResult ExportPerformanceEvaActiveSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Hre_PerformanceEvaActiveEntity, Hre_PerformanceEvaActiveModel>(selectedIds, valueFields, ConstantSql.hrm_hre_sp_get_PerformanceEvaActiveByIds);
        }
        public ActionResult ExportAllPerformanceEvaActiveList([DataSourceRequest] DataSourceRequest request, Hre_PerformanceEvaActiveSearchModel model)
        {
            return ExportAllAndReturn<Hre_PerformanceEvaActiveEntity, Hre_PerformanceEvaActiveModel, Hre_PerformanceEvaActiveSearchModel>(request, model, ConstantSql.hrm_hre_sp_get_PerformanceEvaActive);
        }

        #region lưu và cập nhật quyền phòng ban
        /// <summary>cập nhật quyền phòng ban</summary>
        /// <param name="OrderNumber">OrderNumber của phòng ban</param>
        /// <returns></returns>
        public JsonResult SaveAndUpdatePermissionOrg(int orderNumber)
        {
            //cập nhật quyền phòng ban
            var cat_OrgStructureServices = new Cat_OrgStructureServices();
            string status = string.Empty;
            status = cat_OrgStructureServices.UpdatePermissionOrg(orderNumber);
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Quyen.Quach 23/11/2017 0090313
        public ActionResult GetDurationByContractTypeID(DateTime? dateStart, DateTime? dateEnd, string unitTime)
        {
            if (dateEnd != null && dateStart != null && unitTime != null)
            {
                var contracttype = new Cat_ContractTypeEntity();
                if (unitTime == EnumDropDown.UnitType.E_DAY.ToString())
                {
                    contracttype.ValueTime = Math.Round((double)(dateEnd.Value.Subtract(dateStart.Value).Days / ((365.25 / 12) / 30)));
                }
                else if (unitTime == EnumDropDown.UnitType.E_MONTH.ToString())
                {
                    contracttype.ValueTime = Math.Round((double)(dateEnd.Value.Subtract(dateStart.Value).Days / (365.25 / 12)));
                }
                else if (unitTime == EnumDropDown.UnitType.E_YEAR.ToString())
                {
                    contracttype.ValueTime = Math.Round((double)(dateEnd.Value.Subtract(dateStart.Value).Days / (365.25)));
                }
                return Json(contracttype, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #region [Vinh.Mai - 20171129] Xuat Excel Cat_ApprovedGrade
        public ActionResult ExportAllApprovedGradeList([DataSourceRequest] DataSourceRequest request, Cat_ApprovedGradeSearchModel model)
        {
            return ExportAllAndReturn<Cat_ApprovedGradeEntity, Cat_ApprovedGradeModel, Cat_ApprovedGradeSearchModel>(request, model, ConstantSql.hrm_cat_sp_get_ApprovedGrade);
        }

        public ActionResult ExportApprovedGradeSelected(string selectedIds, string valueFields)
        {
            return ExportSelectedAndReturn<Cat_ApprovedGradeEntity, Cat_ApprovedGradeModel>(selectedIds, valueFields, ConstantSql.hrm_cat_sp_get_ApprovedGradeByIds);
        }
        #endregion

        public ActionResult GetMultiLstDelegateCompanyByCompanyID(string text, Guid? companyID, DateTime? dateStart)
        {
            string status = string.Empty;
            var service = new ActionService(UserLogin, LanguageCode);
            if (companyID != null && dateStart != null)
            {
                var obj = new List<object>();
                obj.AddRange(new object[4]);
                obj[0] = companyID;
                obj[1] = dateStart;
                obj[2] = 1;
                obj[3] = int.MaxValue - 1;
                var result = service.GetData<Cat_DelegateCompanyModel>(obj, ConstantSql.hrm_cat_sp_get_DelegateCompanybyCompanyID, ref status);

                if (!string.IsNullOrEmpty(text))
                {
                    result = result.Where(s => s.ProfileName.Contains(text)).ToList();
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (companyID == null && dateStart != null)
            {
                var obj = new List<object>();
                obj.AddRange(new object[3]);
                obj[0] = dateStart;
                obj[1] = 1;
                obj[2] = int.MaxValue - 1;
                var result = service.GetData<Cat_DelegateCompanyModel>(obj, ConstantSql.hrm_cat_sp_get_DelegateCompanybyDateStart, ref status);

                if (!string.IsNullOrEmpty(text))
                {
                    result = result.Where(s => s.ProfileName.Contains(text)).ToList();
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return GetDataForControl<Cat_DelegateCompanyMultiModel, Cat_DelegateCompanyMultiModel>(text, ConstantSql.hrm_cat_sp_get_DelegateCompany_multi);
            }
        }
    }
}
