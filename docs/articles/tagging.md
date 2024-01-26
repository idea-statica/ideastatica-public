# Version Tagging in Our Repository

### Official Releases vs. Main Branch: 
The commit that represents our official release may not always be the latest one in the main branch. Official releases are marked by tags.

### Tag Names and Messages: 
Each official release is tagged with its version number. This tag not only indicates the release but also contains a message specifying whether it's a beta version or a production release.

### Selecting the Right Version:
As developers, you can continue using our NuGet packages as you have in the past, which are published with each release. Now, to provide additional clarity and synchronization with our repository we add tagging. With every release, the commit on which the version is based is also tagged in our GitHub repository. This means that along with using the NuGet packages, you can also directly reference the specific commit corresponding to the version of the package. While this step is not crucial for all users, it offers an enhanced level of transparency and control, especially useful for understanding the exact state of the codebase at the time of each release. This dual approach of using NuGet packages and referencing repository tags ensures you have full access to the tools necessary for your development tasks, whether you're deploying in a production environment or engaging in testing scenarios.

### Useful commands
- **git tag** for list all tags
- **git checkout 'tag_name'** to checkout to specific commit by tag name
- **git show 'tag_name'** to show tag information like message etc.

