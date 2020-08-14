using Mediatr.Pipelines.Localiztion;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mediatr.Pipelines.Tests.Localization
{
    public class LocalizationResolverTests
    {
        public void Localization_should_work(
            Mock<IStringLocalizer> localizer,
            Model model)
        {
            var sut = new LocalizationResolver(localizer.Object);

            sut.Localize(model);

            localizer.Verify(l => l[nameof(Model.LocalizedProperty)], Times.Once);
            localizer.Verify(l => l["Test"], Times.Once);
            localizer.Verify(l => l[nameof(InnerModel.InnerLocalizedKeyProperty)], Times.Exactly(model.Inner.Count));
            localizer.Verify(l => l["InnerTest"], Times.Exactly(model.Inner.Count));
        }
    }

    public class Model
    {
        [Localize]
        public string LocalizedProperty { get; set; }

        [Localize(Key = "Test")]
        public string LocalizedKeyProperty { get; set; }

        public List<InnerModel> Inner { get; set; }
    }

    public class InnerModel
    {
        [Localize]
        public string InnerLocalizedProperty { get; set; }

        [Localize(Key = "InnerTest")]
        public string InnerLocalizedKeyProperty { get; set; }
    }
}
