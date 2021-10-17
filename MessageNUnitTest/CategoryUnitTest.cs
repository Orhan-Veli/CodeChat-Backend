using NUnit.Framework;
using Category.Dal.Repositories.Abstract;
using Category.Dal.Repositories.Concrete;
using Category.Validation;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Category.Business.Abstract;
using Category.Business.Concrete;
using Category.Dto;
using Category.Dal.Model;
using System;
//using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Category.Controllers;
using Category.Utilities.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace MessageNUnitTest
{
    public class CategoryUnitTest
    {          
        private CategoryDtoValidation _categoryDtoValidation;
        private CategoryDto _categoryDtoTrue;
        private CategoryDto _categoryDtoFalse;
        [SetUp]
        public void Setup()
        {
            _categoryDtoValidation = new CategoryDtoValidation();
            _categoryDtoTrue = new CategoryDto{
                Image = "myimage",
                Name = "orhan"
            };
            _categoryDtoFalse = new CategoryDto
            {
                Name="orhan",
                Image=""
            };
        }

        [Test]
        public async Task CategoryCreateReturnTrue()
        {
            var categoryDtoList = new List<CategoryDto>();
            categoryDtoList.Add(_categoryDtoTrue);       
            var mock = new Mock<ICategoryService>();
            mock.Setup(x=> x.BulkCreateAsync(categoryDtoList).Result.Response).Returns(HttpStatusCode.Ok);
            var controller = new CategoryController(mock.Object);
            var result = await controller.BulkCreateAsync(categoryDtoList);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
        }

        [Test]
        public async Task CategoryCreateReturnFalse()
        {
            var categoryDtoList = new List<CategoryDto>();   
            var mock = new Mock<ICategoryService>();
            mock.Setup(x=> x.BulkCreateAsync(categoryDtoList).Result.Response).Returns(HttpStatusCode.BadRequest);
            var controller = new CategoryController(mock.Object);
            var result = await controller.BulkCreateAsync(categoryDtoList);
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
        }
    }
}