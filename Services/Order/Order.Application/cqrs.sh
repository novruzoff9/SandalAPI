#!/bin/bash

# Kullanıcıdan sınıf adı ve çoğul hali alın
read -p "Sınıf adını girin: " class_name
read -p "Sınıfın çoğul halini girin: " plural_class_name
read -p "Servis adını girin: " service_name

# Kök dizin (ana klasör)
root_directory="$plural_class_name"

# Dizin oluşturma fonksiyonu
create_directory() {
    mkdir -p "$1"
}

# Dosya oluşturma fonksiyonu
create_file() {
    cat > "$1" <<EOL
$2
EOL
}

# Entity dosyasını bulma ve property'leri çıkarma fonksiyonu
get_entity_properties() {
    script_dir=$(dirname "$0")
    entity_file_path="$script_dir/../$service_name.Domain/Entities/${class_name}.cs"
    properties=$(grep -oP 'public \K[^ ]+ [^ ]+(?= { get; set; })' "$entity_file_path" | sed 's/^/public /')
    echo "$properties"
}

# Entity property'lerini al
entity_properties=$(get_entity_properties)

# Komutlar için klasör yapısı
commands_directory="$root_directory/Commands"
create_directory "$commands_directory"
create_directory "$commands_directory/Create${class_name}Command"
create_directory "$commands_directory/Delete${class_name}Command"
create_directory "$commands_directory/Edit${class_name}Command"

# Sorgular için klasör yapısı
queries_directory="$root_directory/Queries"
create_directory "$queries_directory"
create_directory "$queries_directory/Get${plural_class_name}Query"
create_directory "$queries_directory/Get${class_name}Query"

# Create.cs içerik
create_command_file="$commands_directory/Create${class_name}Command/Create${class_name}.cs"
create_file "$create_command_file" "
namespace $service_name.Application.${plural_class_name}.Commands.Create${class_name}Command;

public record Create${class_name}Command(${entity_properties//public /} | tr '\n' ', ') : IRequest<bool>;

public class Create${class_name}CommandHandler : IRequestHandler<Create${class_name}Command, bool>
{
    private readonly IApplicationDbContext _context;

    public Create${class_name}CommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(Create${class_name}Command request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(Create${class_name}));

        var ${class_name,,} = new ${class_name}
        {
            $(echo "$entity_properties" | sed 's/public \([^ ]*\) \([^ ]*\)/\2 = request.\2,/')
        };

        await _context.${plural_class_name}.AddAsync(${class_name,,}, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}
"

# Edit.cs içerik
edit_command_file="$commands_directory/Edit${class_name}Command/Edit${class_name}.cs"
create_file "$edit_command_file" "
namespace $service_name.Application.${plural_class_name}.Commands.Edit${class_name}Command;

public record Edit${class_name}Command(${entity_properties//public /} | tr '\n' ', ') : IRequest<bool>;

public class Edit${class_name}CommandHandler : IRequestHandler<Edit${class_name}Command, bool>
{
    private readonly IApplicationDbContext _context;

    public Edit${class_name}CommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(Edit${class_name}Command request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var ${class_name,,} = await _context.${plural_class_name}.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (${class_name,,} == null) { return false; }

        ${class_name,,} = new ${class_name}
        {
            $(echo "$entity_properties" | sed 's/public \([^ ]*\) \([^ ]*\)/\2 = request.\2,/')
        };

        _context.${plural_class_name}.Update(${class_name,,});

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
"

# Delete.cs içerik
delete_command_file="$commands_directory/Delete${class_name}Command/Delete${class_name}.cs"
create_file "$delete_command_file" "
namespace $service_name.Application.${plural_class_name}.Commands.Delete${class_name}Command;

public record Delete${class_name}Command(string Id) : IRequest<bool>;

public class Delete${class_name}CommandHandler : IRequestHandler<Delete${class_name}Command, bool>
{
    private readonly IApplicationDbContext _context;

    public Delete${class_name}CommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(Delete${class_name}Command request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        var ${class_name,,} = await _context.${plural_class_name}.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (${class_name,,} == null) { return false; }
        _context.${plural_class_name}.Remove(${class_name,,});

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
"

# Get.cs içerik
get_query_file="$queries_directory/Get${plural_class_name}Query/Get${plural_class_name}.cs"
create_file "$get_query_file" "
namespace $service_name.Application.${plural_class_name}.Queries.Get${plural_class_name}Query;

public record Get${plural_class_name}Query : IRequest<List<${class_name}>>;

public class Get${plural_class_name}QueryHandler : IRequestHandler<Get${plural_class_name}Query, List<${class_name}>>
{
    private readonly IApplicationDbContext _context;

    public Get${plural_class_name}QueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<${class_name}>> Handle(Get${plural_class_name}Query request, CancellationToken cancellationToken)
    {
        var ${plural_class_name,,} = await _context.${plural_class_name}.ToListAsync(cancellationToken);
        return ${plural_class_name,,};
    }
}
"

# Get.cs içerik
get_single_query_file="$queries_directory/Get${class_name}Query/Get${class_name}.cs"
create_file "$get_single_query_file" "
namespace $service_name.Application.${plural_class_name}.Queries.Get${class_name}Query;

public record Get${class_name}Query(string Id) : IRequest<${class_name}>;

public class Get${class_name}QueryHandler : IRequestHandler<Get${class_name}Query, ${class_name}>
{
    private readonly IApplicationDbContext _context;

    public Get${class_name}QueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<${class_name}> Handle(Get${class_name}Query request, CancellationToken cancellationToken)
    {
        var ${class_name,,} = await _context.${plural_class_name}.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return ${class_name,,};
    }
}
"

# İşlem tamamlandığında bilgi mesajı
echo "Folder structure created and files for $class_name have been added."