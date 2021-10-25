// <copyright file="RuntimeInvocations.cs" company="Microsoft">
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
    using System.IO;
    using Bdd.Core.Utils;
    using global::Bdd.Core.Hooks;
    using Wpp.StorageAutomation.Tests.Entities;

    public class RuntimeInvocations : RuntimeInvocationBase
    {
        public void CreateDataFile(string text)
        {
            File.WriteAllText(Constants.FilePath.GetFullPath(), text);
        }

        public void DeleteDataFile()
        {
            if (File.Exists(Constants.FilePath.GetFullPath()))
            {
                File.Delete(Constants.FilePath.GetFullPath());
            }
        }
    }
}