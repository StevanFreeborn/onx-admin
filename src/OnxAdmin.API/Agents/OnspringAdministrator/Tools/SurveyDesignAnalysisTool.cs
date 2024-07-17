
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class SurveyDesignAnalysisTool(
  IAttachmentService attachmentService,
  IAnthropicApiClient client
) : IOnspringAdministratorTool
{
  private const string Name = "Run_Survey_Design_Analysis";
  private const string Description = """
    This tool can be used to perform a survey design analysis on a CSV file. 
    It will find the file for the id given, read the file, and provide a summary of the questions that should be added to a survey.
  """;
  private readonly IAttachmentService _attachmentService = attachmentService;
  private readonly IAnthropicApiClient _client = client;

  public async Task<string> RunSurveyDesignAnalysis(
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

      Your task is to analyze the data in the CSV file and provide a summary of the questions that should be added to a survey. The summary should include the following information for each column in the CSV file:
      - The text of the question
      - The type of the question (e.g., multi-select, text, single-select, date, number)
      - The possible answers or options for the question (if applicable)

      Here are the steps to complete this task:

      1. Parse the CSV data and identify the column names.
      2. For each column:
        a. Determine the type of question based on the values. If the column contains multiple types, use the most prevalent one.
        b. Select ONLY from the following types:
            1. Multi Select - Use for questions that require multiple choices from a list of options
            2. Single Line Text - Use for questions that require a single line of text input
            3. Multi-Line Text - Use for questions that require multiple lines of text input
            4. Single Select - Use for questions that require a single choice from a list of options
            5. Date - Use for questions that require a date input
            6. Number - Use for questions that require a numeric input
        c. If the column contains multiple choice questions, list the possible answers or options.
        d. Come up with a concise name for the question based on its content.
      3. If you are given 2 rows of data, treat the first row as headers and use column names to come up with the question text. If you are given a single row of data, generate question text based on the values in the first row.
      4. Based on the data, come up with a name for the survey that will hold these questions. For example, if the questions are about customer satisfaction, you could name the survey ""Customer Satisfaction Survey"".
      5. Construct a response that includes a table of the analysis:
        
        | Question Text                                | Question Type   | Possible Answers                                           |
        |----------------------------------------------|-----------------|------------------------------------------------------------|
        | Which of the following fruits do you like?   | Multi-Select    | Apple, Banana, Orange, Grapes, Mango                       |
        | What are your thoughts on remote work?       | Multi-Line Text | N/A                                                        |
        | What is your favorite season?                | Single-Select   | Spring, Summer, Autumn, Winter                             |
        | When is your birthday?                       | Date            | N/A                                                        |
        | How many books did you read last year?       | Number          | N/A                                                        |
        | What is your name?                           | Single Line Text| N/A                                                        |

        Remember, ONLY use the types provided: Multi Select, Single Line Text, Multi-Line Text, Single Select, Date, Number.
        Remember, ONLY provide the table. DO NOT provide any other information that is NOT requested.
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
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(RunSurveyDesignAnalysis));
  }
}