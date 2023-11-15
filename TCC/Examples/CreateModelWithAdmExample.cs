using ModelExchanger.AnalysisDataModel;
using ModelExchanger.AnalysisDataModel.Contracts;
using ModelExchanger.AnalysisDataModel.Enums;
using ModelExchanger.AnalysisDataModel.Integration.Bootstrapper;
using ModelExchanger.AnalysisDataModel.Libraries;
using ModelExchanger.AnalysisDataModel.Models;
using ModelExchanger.AnalysisDataModel.StructuralElements;
using ModelExchanger.AnalysisDataModel.Subtypes;
using SCIA.OpenAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using Environment = SCIA.OpenAPI.Environment;

namespace TCC_R04.Examples
{
    public class CreateModelWithAdmExample
    {
        private readonly string senPath;
        private readonly string senTempPath;
        private readonly string emptyProjectPath;

        public CreateModelWithAdmExample(string senPath, string senTempPath, string emptyProjectPath)
        {
            this.emptyProjectPath = emptyProjectPath;
            this.senPath = senPath;
            this.senTempPath = senTempPath;
        }

        public void Run()
        {
            (Environment Environment, EsaProject Project) senData = Tools.StartSciaEngineer(senPath, senTempPath, emptyProjectPath);

            var model = CreateModelUsingADMViaOpenApi();

            foreach (IAnalysisObject admObject in model)
            {
                senData.Project.Model.CreateAdmObject(admObject);
            }

            senData.Project.Model.RefreshModel_ToSCIAEngineer();

            senData.Project.CloseProject(SaveMode.SaveChangesNo);

            senData.Environment.Dispose();
        }

        private AnalysisModel CreateModelUsingADMViaOpenApi()
        {
            var model = new AnalysisModel();

            var material = new StructuralMaterial(Guid.NewGuid(), "MAT1", MaterialType.Steel, "S235");
            var crossSection = new StructuralManufacturedCrossSection(Guid.NewGuid(), "CS1", material, "HEA200", FormCode.ISection, DescriptionId.EuropeanIBeam);

            var node1 = new StructuralPointConnection(Guid.NewGuid(), "N1", Length.Zero, Length.Zero, Length.Zero);
            var node2 = new StructuralPointConnection(Guid.NewGuid(), "N2", Length.FromMeters(4), Length.Zero, Length.Zero);
            var beam = new StructuralCurveMember(Guid.NewGuid(), "Beam1", new[]
            {
                new Curve<StructuralPointConnection>(CurveGeometricalShape.Line, new []{ node1, node2 }),
            }, crossSection);


            var bootstrapper = new AnalysisDataModelBootstrapper();

            using (var scope = bootstrapper.CreateThreadedScope())
            {
                var service = scope.GetService<IAnalysisModelService>();

                service.AddItemsToModel(model, new IAnalysisObject[] { material, crossSection, node1, node2, beam });
            }

            model.EnforceModelValidity();
            return model;
        }
    }
}
