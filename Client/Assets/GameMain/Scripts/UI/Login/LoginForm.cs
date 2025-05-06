using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FairyGUI;
using GameMain.Game;
using HoweFramework;

namespace GameMain.UI.Login
{
    /// <summary>
    /// LoginForm逻辑实现。
    /// </summary>
    public partial class LoginForm : FullScreenFormLogicBase
    {
        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        private void OnInitialize()
        {
            m_BtnLogin.onClick.Add(OnLoginButtonClick);
        }

        /// <summary>
        /// 界面销毁回调。
        /// </summary>
        private void OnDispose()
        {
        }

        /// <summary>
        /// 界面打开回调。
        /// </summary>
        public override void OnOpen()
        {
            TbBuffConfig.Instance.DataList.ForEach(buff =>
            {
                Log.Info($"Buff: {buff.Id} {buff.Name}");
            });

            Log.Info($"GetText(demo_text_1): {LocalizationModule.Instance.GetText("demo_text_1")}");
        }

        /// <summary>
        /// 界面关闭回调。
        /// </summary>
        public override void OnClose()
        {
        }

        /// <summary>
        /// 界面更新回调(打开时也会触发)。
        /// </summary>
        public override void OnUpdate()
        {
        }

        /// <summary>
        /// 界面显示回调。
        /// </summary>
        public override void OnVisible()
        {
        }

        /// <summary>
        /// 界面隐藏回调。
        /// </summary>
        public override void OnInvisible()
        {
        }

        /// <summary>
        /// 登录按钮点击回调。
        /// </summary>
        private void OnLoginButtonClick(EventContext context)
        {
            // Request?.SetResponse(CommonResponse.Create(ErrorCode.Success, "Test"));
            // GameApp.Instance.RestartGame();
            // SoundUtility.PlaySound(SoundGroupId.UI, "Assets/GameMain/Sound/sound_demo.wav");
            // WebRequestModule.Instance.PostJsonObject("http://localhost:5006/api/demo/json", new DemoDto() { id = 1001, name = "Test" })
            // .ContinueWith(response =>
            // {
            //     Log.Info($"POST Response: {response.ResponseText}");
            // });

            // WebRequestModule.Instance.Get("https://www.baidu.com").ContinueWith(response =>
            // {
            //     Log.Info($"GET Response: {response.ResponseText}");
            // });

            // const string Template1 = "Hello {name}!";
            // const string Template2 = "Hello {name=LiLei}! Today is {today}. {nokey}";

            // TextUtility.AddGlobalTemplateValue("today", DateTime.Now.DayOfWeek.ToString());

            // var result1 = TextUtility.ParseTemplate(Template1, new Dictionary<string, string>(){
            //     { "name", "HanMeimei" }
            // });
            // var result2 = TextUtility.ParseTemplate(Template2, null);

            // Log.Info($"Template1: {result1}");
            // Log.Info($"Template2: {result2}");

            LoginAsync().Forget();
        }

        private async UniTask LoginAsync()
        {
            using var response = await UIModule.Instance.OpenUIForm(UIFormId.LoginAccountForm).As<LoginAccountResponse>();
            Log.Info($"登录界面返回: Code={response.ErrorCode} Account={response.Account} Password={response.Password}");
        }
        
        // [Serializable]
        // public sealed class DemoDto
        // {
        //     public int id;
        //     public string name;
        // }
    }
}