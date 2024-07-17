
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class DataImportAnalysisTool(
  IAttachmentService attachmentService,
  IAnthropicApiClient client
) : IOnspringAdministratorTool
{
  private const string Name = "Run_Data_Import_Analysis";
  private const string Description = """
    This tool can be used to perform a data import analysis on a CSV file. 
    It will find the file for the id given, read the file, and provide a summary of the fields needed to import the data into an Onspring app.
  """;

  private readonly IAttachmentService _attachmentService = attachmentService;
  private readonly IAnthropicApiClient _client = client;

  public async Task<string> RunDataImportAnalysis(
    [FunctionParameter("The ID of the file to analyze.", required: true)]
    string fileId
  )
  {
    var fileExtension = await _attachmentService.GetAttachmentExtensionAsync(fileId);

    if (fileExtension is not ".csv")
    {
      return "Unable to analyze the file. The file is not a CSV file.";
    }

    var fileContent = await _attachmentService.GetAttachmentLinesAsync(fileId);
    var rows = fileContent.Take(2).ToList();

    var prompt = @$"""
      I will provide you with a CSV file containing data:

      <csv_file>
      {string.Join("\n", rows)}
      </csv_file>

      Your task is to analyze the data in the CSV file and provide a summary of the fields needed to import the data into an Onspring app. The summary should include the following information for each column in the CSV file:
      - The name of the column
      - A brief description of what that column represents
      - The data type of the values in that column

      Here are the steps to complete this task:

      1. Parse the CSV data and identify the column names.
      2. For each column:
        a. Determine the data type by inspecting the values. If the column contains multiple data types, use the most prevalent one.
        b. Select ONLY from the following types:
            1. Date - Use for Date or Date + Time values such as 06-10-2024
            2. List - Use for categorical or label data such as Status, Category, Type, Language, Country, etc.
            4. Text - Use for alphanumeric characters such as HTML markup, json, multi-line text, emails, phone numbers, etc.
            5. Number - Use for any type of numeric value
        b. Come up with a concise description of what the column likely represents based on its name and values.
      3. If you are given 2 rows of data, treat first row as headers and use column names as names. If you are given a single row of data, generate names based on the values in the first row.
      4. Based on the data come up with a name for the app that will hold this data. For example, if the data is about tasks, you could name the app ""Tasks"".
      5. Construct a response that includes a table of the analysis:
        
        | Field Name | Field Description               | Field Type |
        |------------|---------------------------------|------------|
        | name       | The name of the person          | Text       |
        | age        | The age of the person in years  | Number     |
        | city       | The city where the person lives | Text       |
        
      5. Output the table in Markdown format.

      For example, if the input CSV looked like:

      ```
      name,age,city
      Alice,25,New York
      Bob,30,Chicago
      Charlie,35,Houston
      ```

      The expected table summary would be:
      
      | Field Name | Field Description               | Field Type |
      |------------|---------------------------------|------------|
      | name       | The name of the person          | Text       |
      | age        | The age of the person in years  | Number     |
      | city       | The city where the person lives | Text       |

      Remember, ONLY use the types provided: Date, List, Text, Number. DO NOT use any other types.
      Remember, ONLY provide the Table. DO NOT provide any other information.
    """;

    var request = new MessageRequest(
      AnthropicModels.Claude35Sonnet,
      [new(MessageRole.User, [new TextContent(prompt)])]
    );

    var response = await _client.CreateMessageAsync(request);

    if (response.IsFailure)
    {
      return "An error occurred while trying to analyze the file. Please try again later.";
    }

    return response.Value.GetText();
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(RunDataImportAnalysis));
  }
}