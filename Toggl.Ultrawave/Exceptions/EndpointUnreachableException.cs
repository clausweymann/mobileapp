﻿using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Exceptions
{
    public class EndpointUnreachableException : ApiException
    {
        private const string defaultErrorMessage = "Api endpoint could not be reached.";

        internal EndpointUnreachableException(IRequest request, IResponse response)
            : this(request, response, defaultErrorMessage)
        {
        }

        internal EndpointUnreachableException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
