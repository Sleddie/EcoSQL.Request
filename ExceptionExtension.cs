using System.Text;

namespace EcoSQL.Request;

/// <summary>Provides extension methods for exceptions instances.</summary>
public static class ExceptionExtension
{
    /// <param name="exception">The exception for which to get the full message.</param>
    extension(Exception exception)
    {
        /// <summary>Gets the full message of an exception, including inner exceptions.</summary>
        /// <returns>The full message of the exception.</returns>
        public string GetExceptionFullMessage()
        {
            if (exception.InnerException is null)
            {
                return exception.Message;
            }

            StringBuilder fullMessageBuilder = new(exception.Message);
            Exception? innerEx = exception.InnerException;

            while (innerEx is not null)
            {
                fullMessageBuilder.Append($"{Environment.NewLine}\t{innerEx.Message}");
                innerEx = innerEx.InnerException;
            }

            return fullMessageBuilder.ToString();
        }
    }
}