namespace PaymentGateway.Api.ErrorHandler
{
    public static class ExceptoinExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
