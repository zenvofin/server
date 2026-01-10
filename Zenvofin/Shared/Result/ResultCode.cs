namespace Zenvofin.Shared.Result;

public enum ResultCode
{
    Ok = 200,
    Created = 201,
    BadRequest = 400,
    Unauthorized = 401,
    NotFound = 404,
    Locked = 423,
    InternalError = 500,
}