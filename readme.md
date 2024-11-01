
# Local Development Setup

You will need to set up a few things to run the application locally.

## SQL Server

Firstly, you will need to setup a local SQL server, see this [link](https://www.c-sharpcorner.com/article/step-by-step-installation-of-microsoft-sql-server-on-windows-system/) on how to do that.
One thing to note on your SQL server, we are using the SQL Server system administration ("sa") account to connect to it, select Mixed mode for the Authentication Mode when setting up your server, and take note of the password as you will need to set it in your connection string to run the application.
Once your SQL Server is up and running, connect to it using SSMS and create a new database called "ToDoAPIDB". See this [link](https://learn.microsoft.com/en-us/sql/relational-databases/databases/create-a-database?view=sql-server-ver16) on how to do that.

## Dotnet Environment

This is a Dotnet application so you will need to setup your local development environment, this is a good article on how to do that. See this [link](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/tutorials/local-environment)

## Connection String

We use local app secrets to store sensitive configuration for our application. Specifically we are storing a connection string in our application secrets. 
To add the connection string to your local secrets, you may need to enable secret storage. See this [link](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows).

You should have the below in your local secrets file. To add a secret right-click on the ToDoAPI Project and select "Manage User Secrets". 
Then paste in the below (replace YOURPASSWORD with the password you specified for the sa account on your SQL server):
*{
  "ConnectionStrings": { "ToDoDB":"Server[::1],1433;Database=ToDoAPIDB;User=sa;Password=YOURPASSWORD;Pooling=True;Trust Server Certificate=True;"}
}*

## AWS CLI

In order to deploy your lamba application you will need to setup the AWS cli. See this [link](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html)

## .NET CLI Extension

To deploy your application to an AWS Lambda you will need to install the .NET CLI Extension Amazon.Lambda.Tools, run the following command to do this: "dotnet tool install -g Amazon.Lambda.Tools"

## Deploying the Dotnet application to AWS CLI

Follow the below steps:
1. Open a command prompt window, and navigate to the folder containing your Visual Studio .NET Core Lambda project.
2. If prompted, enter the AWS Region (the Region to which your Lambda function will be deployed).
3. Enter "dotnet lambda deploy-function" and press enter.
4. When prompted, enter in the runtime, for our particular application it is "dotnet8"
5. When prompted, enter the name of the function to deploy. It can be a new name or the name of an existing function.
6. When prompted, select an IAM role, you can either use an existing one or create a new one. We will create a new one, select the option that says "Create new IAM Role"
    - First you will need to enter in a name for your role, you can enter in any name
    - Then you will be prompted to select a IAM policy, I suggest using "AWSLambdaBasicExecutionRole"
    - This should complete once its done propagating the role to all regions
7. When prompted, enter in the memory size, our application is basic so 512 MB should suffice, enter in "512" and press enter
8. When prompted, enter in the timeout, I went with 5 seconds, enter in "5" and press enter
9. When prompted, enter in the handler, this is the name of your assembly, in our case it's "ToDoAPI", after entering that in press enter
10. On successful completion, the message New Lambda function created is displayed 
