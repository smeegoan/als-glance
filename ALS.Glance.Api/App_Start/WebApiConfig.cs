﻿using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using ALS.Glance.Api.Models;
using ALS.Glance.Models;
using ALS.Glance.Models.Security.Implementations;

namespace ALS.Glance.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            #region OData Registration

            var builder = new ODataModelBuilder();
            builder.EntitySet<ApiUser>(
              es =>
              {
                  es.HasKey(e => e.UserName);
                  es.Property(e => e.UserName).IsRequired();
                  es.Property(e => e.Email);
                  es.Property(e => e.CreatedOn).IsRequired();
                  es.Property(e => e.UpdatedOn).IsRequired();
                  es.Property(e => e.GivenName);
                  es.Property(e => e.FamilyName);

                  es.Action("ResetPassword");

                  var action = es.Action("ChangePassword");
                  action.Parameter<string>(FieldNames.ApiUser_NewPassword);
                  action.Parameter<string>(FieldNames.ApiUser_CurrentPassword);
              });
            builder.EntitySet<AuthorizationRefresh>(
                es =>
                {
                    es.HasKey(e => e.ApplicationId).HasKey(e => e.RefreshToken);
                    es.Property(e => e.ApplicationId).IsRequired();
                    es.Property(e => e.RefreshToken).IsRequired();
                    es.ComplexProperty(e => e.Authorization).IsOptional();
                }).HasEditLinkForKey(true, e => e.ApplicationId, e => e.RefreshToken);
            builder.EntitySet<AuthorizationRequest>(
                es =>
                {
                    es.HasKey(e => e.ApplicationId).HasKey(e => e.UserName).HasKey(e => e.Password);
                    es.Property(e => e.ApplicationId).IsRequired();
                    es.Property(e => e.UserName).IsRequired();
                    es.Property(e => e.Password).IsRequired();
                    es.ComplexProperty(e => e.Authorization).IsOptional();
                }).HasEditLinkForKey(true, e => e.ApplicationId, e => e.UserName, e => e.Password);
            builder.ComplexType<Authorization>(
                es =>
                {
                    es.Property(e => e.AccessToken).IsRequired();
                    es.Property(e => e.RefreshToken).IsRequired();
                    es.Property(e => e.ExpiresIn).IsRequired();
                    es.Property(e => e.UserName).IsRequired();
                });
            builder.EntitySet<ApiUserRegistration>(
                es =>
                {
                    es.HasKey(e => e.Email);
                    es.Property(e => e.Email).IsRequired();
                    es.Property(e => e.Password).IsRequired();
                    es.Property(e => e.PasswordConfirmation).IsRequired();
                    es.Property(e => e.AcceptsTermsAndConditions).IsRequired();
                    es.HasOptional(e => e.User);
                }).HasEditLinkForKey(true, e => e.Email);
            builder.ComplexType<ApiApplicationUser>(
                es =>
                {
                    es.Property(e => e.Id);
                });
            builder.EntitySet<ApplicationSettings>(
                es =>
                {
                    es.HasKey(e => e.UserId).HasKey(e => e.ApplicationId);
                    es.Property(e => e.UserId).IsRequired();
                    es.Property(e => e.ApplicationId).IsRequired();
                    es.HasRequired(e => e.User);
                    es.ComplexProperty(e => e.Application).IsRequired();
                    es.HasDynamicProperties(e => e.Values);

                    es.Property(e => e.CreatedOn).IsRequired();
                    es.Property(e => e.UpdatedOn).IsRequired();
                }).HasEditLinkForKey(true, e => e.UserId, e => e.ApplicationId);

            builder.EntitySet<Fact,long>(type =>
            {
                type.Property(e => e.AUC);
                type.HasRequired(e => e.Patient);
                type.HasRequired(e => e.Muscle);
                type.HasRequired(e => e.Date);
                type.HasRequired(e => e.Time);
            });
            builder.EntitySet<DMuscle, long>(type =>
            {
                type.Property(e => e.Name);
                type.Property(e => e.Abbreviation);
            });
            builder.EntitySet<DPatient, long>(type =>
            {
                type.Property(e => e.Name);
                type.Property(e => e.PatientId);
                type.Property(e => e.Sex);
                type.Property(e => e.DiagnosedOn);
                type.Property(e => e.BornOn);
            });
            builder.EntitySet<DTime, long>(type =>
            {
                type.Property(e => e.Hour);
                type.Property(e => e.TimeOfDay);
            });
            builder.EntitySet<DDate, long>(type =>
            {
                type.Property(e => e.DayOfWeek);
                type.Property(e => e.Month);
                type.Property(e => e.Weekday);
                type.Property(e => e.Year);
                type.Property(e => e.Date);
                type.Property(e => e.Day);
                type.Property(e => e.DayInMonth);
                type.Property(e => e.DayOfWeekName);
                type.Property(e => e.MonthInYear);
                type.Property(e => e.MonthName);
                type.Property(e => e.Quarter);
                type.Property(e => e.QuarterInYear);
            });
            builder.Namespace = "";
            config.MapODataServiceRoute("ODataRoute", "odata", builder.GetEdmModel());

            #endregion
        }


    }
}