using HoneyRaesAPI.Models;
List<Customer> customers = new List<Customer> {
    new Customer()
    {
        Id = 1,
        Name = "Larry",
        Address = "122 Ruben Rd"
    },
    new Customer()
    {
        Id = 2,
        Name = "Betty",
        Address = "12 Ruby Rd"
    },
    new Customer()
    {
        Id = 3,
        Name = "Sue",
        Address = "111 Belair Rd"
    }
};
List<Employee> employees = new List<Employee> {
    new Employee()
    {
        Id = 1,
        Name = "Bobby",
        Specialty = "Mopper"
    },
    new Employee()
    {
        Id = 2,
        Name = "Mary",
        Specialty = "Cashier"
    }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket> {
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Needs more help with the clean up.",
        Emergency = false,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Needs more help with starting a payment plan.",
        Emergency = true,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "Needs more help with a broken item.",
        Emergency = false,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Needs more help with an injury that occured.",
        Emergency = false,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Needs more help with some clarity of the service.",
        Emergency = false,
        DateCompleted = new DateTime()
    }
};







var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTickets.Remove(serviceTicket);
    return Results.Ok(serviceTicket);
});
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});
app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    
    ticketToComplete.DateCompleted = DateTime.Today;
    
});
app.Run();




