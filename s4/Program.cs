using Amazon.S3;
using Microsoft.Extensions.Options;
using s4.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AwsConfig>(builder.Configuration.GetSection(nameof(AwsConfig)));

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var awsConfig = sp.GetRequiredService<IOptions<AwsConfig>>().Value;
    return new AmazonS3Client(
        awsConfig.AwsAccessKey,
        awsConfig.AwsSecretKey,
        Amazon.RegionEndpoint.APSoutheast1
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
