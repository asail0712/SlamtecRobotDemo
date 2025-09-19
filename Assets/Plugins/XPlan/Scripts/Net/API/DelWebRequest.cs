using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using XPlan.Utility;

namespace XPlan.Net
{
    public class DelWebRequest : WebRequestBase
	{	
		public DelWebRequest()
        {
			
		}

        override protected string GetRequestMethod()
        {
            return UnityWebRequest.kHttpVerbDELETE;
        }
    }
}
