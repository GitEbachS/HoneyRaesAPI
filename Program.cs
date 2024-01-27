using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;
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
    },
    new Employee()
    {
        Id = 3,
        Name = "Billy",
        Specialty = "Sweeper"
    },
    new Employee()
    {
        Id = 4,
        Name = "Dolly",
        Specialty = "Cook"
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
        DateCompleted = null
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Needs more help with starting a payment plan.",
        Emergency = true,
        DateCompleted = null
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = null,
        Description = "Needs more help with a broken item.",
        Emergency = false,
        DateCompleted = null
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Needs more help with an injury that occured.",
        Emergency = false,
        DateCompleted = new DateTime(2022, 12, 11)
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 3,
        EmployeeId = null,
        Description = "Needs more help with some clarity of the service.",
        Emergency = true,
        DateCompleted = null
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
app.MapGet("/servicetickets/emergencies", () =>
{
    List <ServiceTicket> filteredTickets = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null).ToList();
    if (filteredTickets == null)
    {
        return Results.NotFound();
    }


    return Results.Ok(filteredTickets);
});

app.MapGet("/servicetickets/unassigned", () =>
{
    List<ServiceTicket> unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    if (unassignedTickets == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(unassignedTickets);
});

app.MapGet("/servicetickets/closed", () =>
{
    DateTime thisDate = DateTime.Today;
    DateTime thisDateLasttYear;
    thisDateLasttYear = thisDate.AddYears(-1);



    List <ServiceTicket> closedOneYearTickets = serviceTickets.Where(st => st.DateCompleted < thisDateLasttYear).ToList();
    List<Customer> customerPeople = new List<Customer>();
    foreach (ServiceTicket ticket in closedOneYearTickets)
    {
        Customer customerNames = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
       customerPeople.Add(customerNames);
    }
   
    return Results.Ok(customerPeople);
});



app.MapGet("/employees/available", () =>
{
   var assignedEmployees = serviceTickets.Where(st => st.EmployeeId.HasValue).Select(st => st.EmployeeId.Value).ToList();
    var availableEmployees = employees.Where(e => !assignedEmployees.Contains(e.Id)).ToList();

    return Results.Ok(availableEmployees);
});

app.MapGet("/customers/assigned", () =>
{
    List<Customer> customersList = new();
    List<ServiceTicket> tickets = serviceTickets.Where(ticket => ticket.EmployeeId != null).ToList();

    foreach (ServiceTicket ticket in tickets)
    {
        var cust = customers.Where(x => x.Id == ticket.CustomerId).FirstOrDefault();
        customersList.Add(cust);
    }
    return Results.Ok(customersList);
});

app.MapGet("/customer/bytheemployee", (int id) =>
{
    var tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    List<Customer> customerList = new();
    foreach(ServiceTicket ticket in tickets)
    {
        Customer customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        customerList.Add(customer);
    }
    return Results.Ok(customerList.Distinct().ToList());
});

app.MapGet("/employees/ofthemonth", () =>
{
   DateTime lastMonth = DateTime.Now.AddDays(-30);
    var thisTicket = serviceTickets.Where(st => st.DateCompleted > lastMonth && st.DateCompleted < DateTime.Now).Select(t => t.EmployeeId.Value).ToList();
    var result = thisTicket.GroupBy(x => x).OrderByDescending(x => x.Count()).ThenBy(x => x.Key).SelectMany(x => x).ToList();
    var answer = employees.Where(e => e.Id == result.First());

    return Results.Ok(answer);

});


app.MapGet("/servicetickets/completedfirst", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(t => t.DateCompleted != null).OrderBy(t => t.DateCompleted.Value).ToList();
    return Results.Ok(completedTickets);

});

app.MapGet("/servicetickets/order", () =>
{
    List<ServiceTicket> incompleteTickets = serviceTickets.Where(t => t.DateCompleted == null).OrderBy(x => x.Emergency ? 0 : 1).ThenBy(s => s.EmployeeId == null ? 0 : 1).ToList();
    return Results.Ok(incompleteTickets);
});
app.Run();

/*Create an endpoint to return all tickets that are incomplete, in order first by whether they are emergencies,
 * then by whether they are assigned or not (unassigned first)..*/





