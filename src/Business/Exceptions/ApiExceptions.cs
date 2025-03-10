using System;

namespace Business.Exceptions;

public static class ApiExceptions
{
    public static ApiException OE001 => new ApiException("OE001", "Order not found.");
}
