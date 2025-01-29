﻿namespace GoldHelpers.Middleware
{
    public class APIResponse
    {
        public APIResponse(int? statusCode = 200, string? message = "", string? data = null)
        {
            StatusCode = statusCode;
            Message = string.IsNullOrEmpty(message) ? GetDefaultMessageForStatusCode(statusCode ?? 200) : message;
            Data = data;
        }

        public int? StatusCode { get; set; } = 200;
        public string? Data { get; set; }
        public string? Message { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "عملیات با موفقیت انجام یافت",
                201 => "کد تائیدیه صحیح نمی باشد و یا منقضی شده است",
                400 => "درخواست قابل پردازش نمی باشد!",
                401 => "درخواست فاقد اعتبار معتبر می باشد!",
                404 => "داده یافت نشد!",
                500 => "سیستم با مشکل مواجه شده است، لطفا با پشتیبانی تماس بگیرید.",
                501 => "زمان تأییدیه به پایان رسیده است!",
                503 => "نام کاربری یا رمز عبور صحیح نمی باشد",
                504 => "رمز عبور از پروتکل اعلامی تبعیت نمی کند",
                _ => null
            };
        }
    }
}
