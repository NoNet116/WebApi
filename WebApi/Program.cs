using BLL.Extensions;
using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // ��� ����� ��� ��������� Swagger-����������
builder.Services.AddSwaggerGen(); // ������������ Swagger ���������

//�����������
builder.Services.AddAutoMapper(
    typeof(BLL.Mapper.BLLMappingProfile),
    typeof(PLLMappingProfile)
);
string connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("no string connection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connection));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddIdentity<DAL.Entities.User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 2;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IRoleService, RoleService>().AddIdentityRoles();


//End
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ��������� swagger.json
    app.UseSwaggerUI(); // ���������� ��������� Swagger � ��������
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
