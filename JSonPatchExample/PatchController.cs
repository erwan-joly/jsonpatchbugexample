using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace JSonPatchExample
{
    [Route("api/[controller]")]
    public class PatchController
    {
        [HttpPatch]
        public void JsonPatchWithModelState(
            [FromBody] JsonPatchDocument<SmallObject> patchDoc)
        {
            Console.WriteLine(patchDoc != null ? "all good here" : "something went wrong");
        }
    }
}
