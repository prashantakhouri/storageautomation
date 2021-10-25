// <copyright file="CommonSteps.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Wpp.StorageAutomation.Tests.StepDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bdd.Core.Utils;

    using global::Bdd.Core.Web.StepDefinitions;
    using TechTalk.SpecFlow;

    using Wpp.StorageAutomation.Tests.Executors;

    [Binding]
    public class CommonSteps : WebStepDefinitionBase
    {
        [StepDefinition(@"I send (a dictionary "".*"")")]
        public void WhenISendADictionaryKVKV(Dictionary<string, string> dict)
        {
            Console.WriteLine(dict);
        }
    }
}
