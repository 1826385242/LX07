using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class WorkController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public WorkController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Json(DAL.WorkInfo.Instance.GetCount());
        }
        [HttpGet("new")]
        public ActionResult GetNew()
        {
            var result = DAL.WorkInfo.Instance.GetNew();
            if (result.Count() != 0)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("num wei 0"));
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var result = DAL.WorkInfo.Instance.GetModel(id);
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("id can't find"));
        }
        [HttpPost]
        public ActionResult Post([FromBody]Model.WorkInfo workInfo)
        {
            workInfo.recommend = "否";
            workInfo.workVerify = "待审核";
            workInfo.uploadTime = DateTime.Now;
            try
            {
                int n = DAL.WorkInfo.Instance.Add(workInfo);
                return Json(Result.Ok("success", n));
            }
            catch(Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key"))
                    return Json(Result.Ok(ex.Message));
                else
                    return Json(Result.Ok(ex.Message));
            }
        }
        [HttpPut]
        public ActionResult Put([FromBody]Model.WorkInfo workInfo)
        {
            workInfo.recommend = "否";
            workInfo.workVerify = "待审核";
            workInfo.uploadTime = DateTime.Now;
            try
            {
                int n = DAL.WorkInfo.Instance.Add(workInfo);
                return Json(Result.Ok("success", n));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key"))
                    return Json(Result.Ok(ex.Message));
                else
                    return Json(Result.Ok(ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var n = DAL.WorkInfo.Instance.Delete(id);
                if (n != 0)
                    return Json(Result.Ok("delete success"));
                else
                    return Json(Result.Err("workid can't find"));
            }
            catch(Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
        [HttpPost("count")]
        public ActionResult getCount([FromBody]int[] activityIds)
        {
            return Json(DAL.WorkInfo.Instance.GetCount(activityIds));
        }
        [HttpPost("page")]
        public ActionResult getPage([FromBody] Model.WorkPage page)
        {
            var result = DAL.WorkInfo.Instance.GetPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("num wei 0"));
            else
                return Json(Result.Ok(result));
        }
        [HttpGet("findCount")]
        public ActionResult getFindCount(string findName)
        {
            if (findName == null) findName = "";
            return Json(DAL.WorkInfo.Instance.GetFindCount(findName));
        }
        [HttpGet("myCount")]
        public ActionResult getMyCount(string userName)
        {
            if (userName == null) userName = "";
            return Json(DAL.WorkInfo.Instance.GetMyCount(userName));
        }
        [HttpPost("findPage")]
        public ActionResult getFindPage([FromBody]Model.WorkFindPage page)
        {
            if (page.workName == null) page.workName = "";
            var result = DAL.WorkInfo.Instance.GetFindPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("num wei 0"));
            else
                return Json(Result.Ok(result));
        }
        [HttpPost("myPage")]
        public ActionResult getMyPage([FromBody]Model.WorkFindPage page)
        {
            if (page.workName == null) page.workName = "";
            var result = DAL.WorkInfo.Instance.GetFindPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("num wei 0"));
            else
                return Json(Result.Ok(result));
        }
        [HttpPut("{id}")]
        public ActionResult upImg(string id ,List<IFormFile> files)
        {
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img", "Work");
            var fileName = $"{path}/{id}";
            try
            {
                var ext = DAL.Upload.Instance.UpImg(files[0], fileName);
                if (ext == null)
                    return Json(Result.Err("please send picture"));
                else
                {
                    var file = $"img/Work/{id}{ext}";
                    Model.WorkInfo workInfo = new Model.WorkInfo();
                    if(id.StartsWith("i"))
                    {
                        workInfo.workId = int.Parse(id.Substring(1));
                        workInfo.workIntroduction = file;
                    }
                    else
                    {
                        workInfo.workId = int.Parse(id);
                        workInfo.workIntroduction = file;
                    }
                    var n = DAL.WorkInfo.Instance.UpdateImg(workInfo);
                    if (n > 0)
                        return Json(Result.Ok("success", file));
                    else
                        return Json(Result.Err("are you ready?"));
                }
            }
            catch(Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
    }
}
