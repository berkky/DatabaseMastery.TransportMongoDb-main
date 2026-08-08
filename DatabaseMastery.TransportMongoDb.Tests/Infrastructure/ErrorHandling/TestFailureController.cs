using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.ErrorHandling;

public sealed class TestFailureController : Controller
{
    public const string ExceptionMessage = "TEST_EXCEPTION_DETAIL_MUST_NOT_LEAK";

    public IActionResult Index()
    {
        throw new InvalidOperationException(ExceptionMessage);
    }
}
