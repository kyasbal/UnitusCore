using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitusCore.Controllers;

namespace UnitusCore.Validator
{
    public class Validator<T>where T:UnitusApiController
    {
        private readonly T _controller;

        public Validator(T controller)
        {
            _controller = controller;
        }


    }

    public class CircleValidator
    {

//        public ValidatorResultBase ValidateAddCircle(AddCircleRequest req)
//        {
//            
//        }
    }

    public class ValidatorResultBase
    {
        public ValidatorResultBase(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public ValidatorResultBase()
        {

        }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}