# BestPracticesSourceGeneratorsDemo
An example demonstraing a few useful practices when working with source generators

The example is focused on advices for working with source generators that can be checked one by one in commit history such as
- Switching from syntax to semantics as early as possible
- Usage of context.CancellationToken
- Using ISyntaxReceiver to reduce the generator's working time
- Providing attributes that should be used in target projects to configure a generator from the generator itself
- Configuring a generator via MSBuild properties
- Providing a generator with access to additional files it might need
- Emitting diagnostics when generator encounters a problem
- Lifting CS8785 warning indicating a generator failure in the target project to an error
- Warning about potential malicious generators
