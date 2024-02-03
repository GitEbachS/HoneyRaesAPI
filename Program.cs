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
    },
     new Customer()
    {
        Id = 4,
        Name = "Shari",
        Address = "1 Ray Rd"
    },
      new Customer()
    {
        Id = 5,
        Name = "Lambert",
        Address = "12 Sea Rd"
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
    },
    new Employee()
    {
        Id = 5,
        Name = "Patricia",
        Specialty = "Plumber"
    },
    new Employee()
    {
        Id = 6,
        Name = "Jimmy",
        Specialty = "Encourager"
    },
    new Employee()
    {
        Id = 7,
        Name = "Kelly",
        Specialty = "Cleaner"
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
        EmployeeId = 2,
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



app.MapGet("/api/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.Select(x => new Employee { Id = x.Id, Name = x.Name, Specialty = x.Specialty })
    .FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.Select(x => new Customer { Id = x.Id, Name = x.Name, Address = x.Address })
    .FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/api/employees", () =>
{
    return employees;
});

app.MapGet("/api/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/api/customers", () =>
{
    return customers;
});

app.MapGet("/api/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});

app.MapPost("/api/servicetickets", (ServiceTicket serviceTicket) =>
{
serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTickets.Remove(serviceTicket);
    return Results.Ok(serviceTicket);
});
app.MapPut("/api/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
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

app.MapPut("/api/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    
    ticketToComplete.DateCompleted = DateTime.Today;
    
});
app.MapGet("/api/servicetickets/emergencies", () =>
{
    List <ServiceTicket> filteredTickets = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null).ToList();
    if (filteredTickets == null)
    {
        return Results.NotFound();
    }


    return Results.Ok(filteredTickets);
});

app.MapGet("/api/servicetickets/unassigned", () =>
{
    List<ServiceTicket> unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    if (unassignedTickets == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(unassignedTickets);
});

app.MapGet("/api/servicetickets/closed", () =>
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



app.MapGet("/api/employees/available", () =>
{
   var assignedEmployees = serviceTickets.Where(st => st.EmployeeId.HasValue).Select(st => st.EmployeeId.Value).ToList();
    var availableEmployees = employees.Where(e => !assignedEmployees.Contains(e.Id)).ToList();

    return Results.Ok(availableEmployees);
});

app.MapGet("/api/customers/assigned", () =>
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
app.MapPut("/api/servicetickets/{id}/assign", (int id) =>
    {
        ServiceTicket ticketToAssign = serviceTickets.FirstOrDefault(st => st.Id == id);
        var assignedEmployees = serviceTickets.Where(st => st.EmployeeId.HasValue).Select(st => st.EmployeeId.Value).ToList();
        var availableEmployees = employees.Where(e => !assignedEmployees.Contains(e.Id)).ToList();
        Employee firstEmp = availableEmployees.FirstOrDefault();
        ticketToAssign.Employee = availableEmployees.FirstOrDefault();
        ticketToAssign.EmployeeId = firstEmp.Id;
    
       
        return Results.Ok(ticketToAssign);
    });

app.MapGet("/api/customer/bytheemployee", (int id) =>
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

app.MapGet("/api/employees/ofthemonth", () =>
{
   DateTime lastMonth = DateTime.Now.AddDays(-30);
    var thisTicket = serviceTickets.Where(st => st.DateCompleted > lastMonth && st.DateCompleted < DateTime.Now).Select(t => t.EmployeeId.Value).ToList();
    var result = thisTicket.GroupBy(x => x).OrderByDescending(x => x.Count()).ThenBy(x => x.Key).SelectMany(x => x).ToList();
    var answer = employees.Where(e => e.Id == result.First());

    return Results.Ok(answer);

});


app.MapGet("/api/servicetickets/completedfirst", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(t => t.DateCompleted != null).OrderBy(t => t.DateCompleted.Value).ToList();
    return Results.Ok(completedTickets);

});

app.MapGet("/api/servicetickets/order", () =>
{
    List<ServiceTicket> incompleteTickets = serviceTickets.Where(t => t.DateCompleted == null).OrderBy(x => x.Emergency ? 0 : 1).ThenBy(s => s.EmployeeId == null ? 0 : 1).ToList();
    return Results.Ok(incompleteTickets);
});
app.Run();







