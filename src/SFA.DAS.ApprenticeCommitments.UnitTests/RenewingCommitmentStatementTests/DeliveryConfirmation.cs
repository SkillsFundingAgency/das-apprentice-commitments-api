﻿using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.UnitTests.RenewingRevisionTests
{
    public class DeliveryModelConfirmation
    {
        Fixture _f = new Fixture();
        private Revision _existingRevision;
        private Apprenticeship _apprenticeship;
        private long _commitmentsApprenticeshipId;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApprenticeshipId = _f.Create<long>();
            _existingRevision = _f.Create<Revision>();
            _existingRevision.SetProperty(p => p.CommitmentsApprenticeshipId, _commitmentsApprenticeshipId);
            _apprenticeship = new Apprenticeship(_existingRevision);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_delivery_section_confirmation_status_is_not_set_Then_delivery_section_remains_not_set_regardless_of_data_changes(bool withSameData)
        {
            _existingRevision.SetProperty(p => p.HowApprenticeshipDeliveredCorrect, null);

            var details = withSameData ? _existingRevision.Details.Clone() : _f.Create<ApprenticeshipDetails>();

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.Revisions.Last().HowApprenticeshipDeliveredCorrect.Should().BeNull();
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void When_delivery_section_confirmation_status_is_set_Then_delivery_section_does_not_change_status_regardless_of_data_changes_as_long_as_delivery_is_the_same(bool confirmationStatus, bool withSameData)
        {
            _existingRevision.SetProperty(p => p.HowApprenticeshipDeliveredCorrect, confirmationStatus);
            var details = withSameData ? _existingRevision.Details.Clone() : _f.Create<ApprenticeshipDetails>();
            details.SetProperty(p => p.DeliveryModel, _existingRevision.Details.DeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);
            
            _apprenticeship.Revisions.Last().HowApprenticeshipDeliveredCorrect.Should().Be(confirmationStatus);
        }

        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.PortableFlexiJob)]
        public void When_delivery_section_confirmation_status_is_set_Then_delivery_section_confirmation_does_change_when_delivery_model_is_changed(DeliveryModel existingDeliveryModel, DeliveryModel newDeliveryModel)
        {
            _existingRevision.SetProperty(p => p.HowApprenticeshipDeliveredCorrect, true);
            _existingRevision.Details.SetProperty(p => p.DeliveryModel, existingDeliveryModel);
            var details = _existingRevision.Details.Clone();
            details.SetProperty(p => p.DeliveryModel, newDeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _apprenticeship.Revisions.Last().HowApprenticeshipDeliveredCorrect.Should().BeNull();
        }


        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.PortableFlexiJob)]
        public void When_delivery_model_changed_change_of_circumstances_employer_correct_null(DeliveryModel existingDeliveryModel, DeliveryModel newDeliveryModel)
        {

            _existingRevision.Details.SetProperty(p => p.DeliveryModel, existingDeliveryModel);
            var details = _existingRevision.Details.Clone();
            details.SetProperty(p => p.DeliveryModel, newDeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _existingRevision.EmployerCorrect.Should().Be(null);
        }



        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.PortableFlexiJob)]
        public void When_delivery_model_changed_change_of_circumstances_training_provider_null(DeliveryModel existingDeliveryModel, DeliveryModel newDeliveryModel)
        {

            _existingRevision.Details.SetProperty(p => p.DeliveryModel, existingDeliveryModel);
            var details = _existingRevision.Details.Clone();
            details.SetProperty(p => p.DeliveryModel, newDeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _existingRevision.TrainingProviderCorrect.Should().Be(null);
        }



        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.PortableFlexiJob)]
        public void When_delivery_model_changed_change_of_circumstances_apprentice_details_null(DeliveryModel existingDeliveryModel, DeliveryModel newDeliveryModel)
        {

            _existingRevision.Details.SetProperty(p => p.DeliveryModel, existingDeliveryModel);
            var details = _existingRevision.Details.Clone();
            details.SetProperty(p => p.DeliveryModel, newDeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _existingRevision.ApprenticeshipDetailsCorrect.Should().Be(null);
        }



        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.PortableFlexiJob)]
        public void When_delivery_model_changed_change_of_circumstances_how_app_delivered_correct_null(DeliveryModel existingDeliveryModel, DeliveryModel newDeliveryModel)
        {

            _existingRevision.Details.SetProperty(p => p.DeliveryModel, existingDeliveryModel);
            var details = _existingRevision.Details.Clone();
            details.SetProperty(p => p.DeliveryModel, newDeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _existingRevision.HowApprenticeshipDeliveredCorrect.Should().Be(null);
        }



        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.FlexiJobAgency)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.Regular)]
        [TestCase(DeliveryModel.FlexiJobAgency, DeliveryModel.PortableFlexiJob)]
        public void When_delivery_model_changed_change_of_circumstances_roles_correct_null(DeliveryModel existingDeliveryModel, DeliveryModel newDeliveryModel)
        {

            _existingRevision.Details.SetProperty(p => p.DeliveryModel, existingDeliveryModel);
            var details = _existingRevision.Details.Clone();
            details.SetProperty(p => p.DeliveryModel, newDeliveryModel);

            _apprenticeship.Revise(_commitmentsApprenticeshipId, details, DateTime.Now);

            _existingRevision.RolesAndResponsibilitiesConfirmations.Should().Be(null);
        }






    }
}