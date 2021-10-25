// <copyright file="Transformations.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Wpp.StorageAutomation.Tests
{
    using System.Collections.Generic;
    using TechTalk.SpecFlow;

    [Binding]
    public class Transformations
    {
        public Transformations(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.FeatureContext = featureContext;
            this.ScenarioContext = scenarioContext;
        }

        protected FeatureContext FeatureContext { get; }

        protected ScenarioContext ScenarioContext { get; }

        [StepArgumentTransformation(@"a dictionary ""(.*)""")]
        public Dictionary<string, string> ADictionary(string dictStr)
        {
            // add some logic to convert the incoming string to required return type
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["TestData"] = "TestDataSample1";
            return dict;
        }
    }
}
