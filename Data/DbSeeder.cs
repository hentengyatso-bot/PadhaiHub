using PadhaiHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Data;

// Runs once at start-up and fills in the data the site needs to work
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Admin account
        if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            var admin = new User
            {
                FullName = "Site Administrator",
                Email = "admin@padhaihub.com",
                Role = "Admin"
            };
            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, "Admin@123");
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        // 2. Categories
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Programming", Description = "Coding and software development" },
                new Category { Name = "Mathematics", Description = "Algebra, statistics and problem solving" },
                new Category { Name = "Science", Description = "Physics, chemistry and biology" });
            await context.SaveChangesAsync();
        }

        // 3. Sample members
        if (!await context.Users.AnyAsync(u => u.Role == "Member"))
        {
            var hasher = new PasswordHasher<User>();
            var members = new List<User>
            {
                new() { FullName = "Aisha Bello", Email = "aisha.bello@example.com", Role = "Member" },
                new() { FullName = "Daniel Osei", Email = "daniel.osei@example.com", Role = "Member" },
                new() { FullName = "Priya Nair", Email = "priya.nair@example.com", Role = "Member" },
                new() { FullName = "Tom Walsh", Email = "tom.walsh@example.com", Role = "Member" },
                new() { FullName = "Maria Gomez", Email = "maria.gomez@example.com", Role = "Member" },
            };
            foreach (var m in members)
                m.PasswordHash = hasher.HashPassword(m, "Member@123");
            context.Users.AddRange(members);
            await context.SaveChangesAsync();
        }

        // 4. Courses with lessons and quizzes
        if (!await context.Courses.AnyAsync())
        {
            var programming = await context.Categories.FirstAsync(c => c.Name == "Programming");
            var maths = await context.Categories.FirstAsync(c => c.Name == "Mathematics");
            var science = await context.Categories.FirstAsync(c => c.Name == "Science");

            var python = new Course
            {
                Title = "Introduction to Python Programming",
                Description = "A first course in Python for people who have never written a line of code. You will set up a Python environment, learn the core syntax, and write small programs that read input, make decisions and repeat work automatically.",
                Level = "Beginner",
                CategoryId = programming.CategoryId,
                Lessons = new List<Lesson>
                {
                    new() { Title = "Variables and Data Types", LessonOrder = 1, Content = "Python stores values in variables without needing explicit types. Assign a number and it becomes an int or float; assign text and it becomes a string.\n\nTry:\nage = 25\nname = \"Priya\"\nprice = 19.99\n\nUse type() to inspect: type(age) returns <class 'int'>. Strings join with + or f-strings: f\"{name} is {age} years old\"." },
                    new() { Title = "Control Flow with If and Loops", LessonOrder = 2, Content = "if, elif and else choose a path based on a condition. for and while repeat work.\n\nA for loop over range(5) runs five times, counting 0 to 4. A while loop repeats while its condition stays true.\n\nWatch indentation: Python uses it to mark blocks, so a stray tab produces an IndentationError." },
                    new() { Title = "Functions and Scope", LessonOrder = 3, Content = "A function bundles code under a name. Define with def, pass parameters, return a value.\n\ndef add(a, b):\n    return a + b\n\nVariables inside a function are local and disappear afterwards. Variables outside are global and can be read anywhere; use the global keyword to modify them." },
                    new() { Title = "Lists and Dictionaries", LessonOrder = 4, Content = "A list holds ordered items with index access; a dictionary holds key-value pairs.\n\nBoth are mutable. Loop over a list with for item in my_list. Loop over a dictionary with for key, value in my_dict.items()." },
                    new() { Title = "Reading and Writing Files", LessonOrder = 5, Content = "open() returns a file object; the with statement closes it automatically even if an error occurs.\n\nwith open(\"notes.txt\", \"w\") as f:\n    f.write(\"Hello, file!\")\n\nModes: \"r\" read, \"w\" overwrite, \"a\" append." },
                },
                Quizzes = new List<Quiz>
                {
                    new()
                    {
                        Title = "Python Basics Quiz",
                        Description = "Check your understanding of variables, control flow and functions.",
                        Questions = new List<Question>
                        {
                            new() { QuestionText = "Which keyword starts a function definition in Python?", OptionA = "func", OptionB = "def", OptionC = "function", OptionD = "define", CorrectOption = "B" },
                            new() { QuestionText = "What does this print: for i in range(3): print(i)", OptionA = "1 2 3", OptionB = "0 1 2", OptionC = "0 1 2 3", OptionD = "1 2", CorrectOption = "B" },
                            new() { QuestionText = "Which type stores key-value pairs?", OptionA = "list", OptionB = "tuple", OptionC = "dictionary", OptionD = "string", CorrectOption = "C" },
                            new() { QuestionText = "What happens to a variable created inside a function after it returns?", OptionA = "Becomes global", OptionB = "Saved to disk", OptionC = "Destroyed", OptionD = "Doubles", CorrectOption = "C" },
                            new() { QuestionText = "Which file mode appends to the end?", OptionA = "\"r\"", OptionB = "\"w\"", OptionC = "\"x\"", OptionD = "\"a\"", CorrectOption = "D" },
                        }
                    }
                }
            };

            var javascript = new Course
            {
                Title = "Web Development with JavaScript",
                Description = "Move beyond static HTML: make pages respond to clicks, fetch data and update without page reload. Work with the DOM, handle events, and call APIs.",
                Level = "Intermediate",
                CategoryId = programming.CategoryId,
                Lessons = new List<Lesson>
                {
                    new() { Title = "The DOM and Events", LessonOrder = 1, Content = "The DOM is the browser's live representation of HTML. document.querySelector(\"h1\") finds an element; .textContent and .style modify it.\n\nEvents connect actions to code:\nbutton.addEventListener(\"click\", handleClick);\n\nPrefer addEventListener over inline onclick." },
                    new() { Title = "Arrays and Objects", LessonOrder = 2, Content = "map(), filter() and reduce() replace most for loops.\n\nconst doubled = numbers.map(n => n * 2);\n\nObjects group data: const course = { title: \"JavaScript\", level: \"Intermediate\" };\nDestructure: const { title, level } = course;" },
                    new() { Title = "Async JavaScript", LessonOrder = 3, Content = "A Promise represents a value not yet ready: pending, then fulfilled or rejected.\n\nfetchData().then(r => console.log(r)).catch(e => console.error(e));\n\nasync/await reads like sync code:\n\nasync function load() {\n    const r = await fetchData();\n    console.log(r);\n}\n\nWrap await in try/catch." },
                    new() { Title = "Fetch and APIs", LessonOrder = 4, Content = "fetch() sends HTTP and returns a Promise<Response>.\n\nconst res = await fetch(\"https://api.example.com/courses\");\nconst data = await res.json();\n\nAlways check res.ok — fetch only rejects on network failure, not HTTP errors." },
                },
                Quizzes = new List<Quiz>
                {
                    new()
                    {
                        Title = "JavaScript Fundamentals Quiz",
                        Description = "DOM, arrays and async basics.",
                        Questions = new List<Question>
                        {
                            new() { QuestionText = "Which method attaches a click handler?", OptionA = "onpress()", OptionB = "addEventListener(\"click\", handler)", OptionC = "click = handler", OptionD = "attach(\"click\")", CorrectOption = "B" },
                            new() { QuestionText = "Which array method returns a new array?", OptionA = "push()", OptionB = "sort()", OptionC = "map()", OptionD = "splice()", CorrectOption = "C" },
                            new() { QuestionText = "What does await do inside async?", OptionA = "Skips line", OptionB = "Pauses until Promise settles", OptionC = "Converts to string", OptionD = "Runs twice", CorrectOption = "B" },
                            new() { QuestionText = "Why check response.ok?", OptionA = "fetch only rejects on network failure", OptionB = "Compresses response", OptionC = "Required", OptionD = "Converts JSON", CorrectOption = "A" },
                            new() { QuestionText = "What does fetch() return?", OptionA = "JSON directly", OptionB = "Promise<Response>", OptionC = "String", OptionD = "Headers", CorrectOption = "B" },
                        }
                    }
                }
            };

            var algebra = new Course
            {
                Title = "Algebra Foundations",
                Description = "Build solid algebra skills: solving equations, inequalities and reading graphs. Ideal for building confidence without skipping steps.",
                Level = "Beginner",
                CategoryId = maths.CategoryId,
                Lessons = new List<Lesson>
                {
                    new() { Title = "Solving Linear Equations", LessonOrder = 1, Content = "3x + 5 = 20 has one unknown. Subtract 5, divide by 3: x = 5.\n\nAlways check: 3(5) + 5 = 20. This catches arithmetic slips." },
                    new() { Title = "Working with Inequalities", LessonOrder = 2, Content = "Inequalities use < > \u2264 \u2265. Solve like equations except: multiplying or dividing by a negative flips the sign.\n\n-2x < 8 becomes x > -4, not x < -4." },
                    new() { Title = "Graphing Linear Functions", LessonOrder = 3, Content = "y = mx + b graphs as a line. m is slope (rise/run), b is y-intercept.\n\nPlot b first, then use slope to find another point. Negative slope falls right-to-left." },
                    new() { Title = "Systems of Equations", LessonOrder = 4, Content = "Two equations, same variables. Solve by substitution or elimination.\n\nGraphically, the solution is where lines cross. Parallel = no solution. Same line = infinite solutions." },
                },
                Quizzes = new List<Quiz>
                {
                    new()
                    {
                        Title = "Algebra Basics Quiz",
                        Description = "Equations, inequalities and graphing.",
                        Questions = new List<Question>
                        {
                            new() { QuestionText = "Solve 2x + 4 = 12", OptionA = "x = 4", OptionB = "x = 6", OptionC = "x = 8", OptionD = "x = 2", CorrectOption = "A" },
                            new() { QuestionText = "When flip an inequality sign?", OptionA = "Adding positive", OptionB = "Multiplying/dividing by negative", OptionC = "Never", OptionD = "Variable on left", CorrectOption = "B" },
                            new() { QuestionText = "In y = mx + b, what is b?", OptionA = "Slope", OptionB = "x-intercept", OptionC = "y-intercept", OptionD = "Solution", CorrectOption = "C" },
                            new() { QuestionText = "Parallel lines mean?", OptionA = "Infinite solutions", OptionB = "One solution", OptionC = "No solution", OptionD = "Same line", CorrectOption = "C" },
                            new() { QuestionText = "Which method cancels a variable?", OptionA = "Substitution", OptionB = "Elimination", OptionC = "Graphing", OptionD = "Factoring", CorrectOption = "B" },
                        }
                    }
                }
            };

            var statistics = new Course
            {
                Title = "Introduction to Statistics",
                Description = "Summarise datasets, measure spread, and reason about probability and the normal distribution.",
                Level = "Intermediate",
                CategoryId = maths.CategoryId,
                Lessons = new List<Lesson>
                {
                    new() { Title = "Mean, Median and Mode", LessonOrder = 1, Content = "Mean = sum / count (sensitive to outliers).\nMedian = middle value (resistant to outliers).\nMode = most frequent.\n\nIncome is usually reported as median for this reason." },
                    new() { Title = "Standard Deviation and Variance", LessonOrder = 2, Content = "Variance = average squared distance from mean. Standard deviation = sqrt(variance), in original units.\n\nSmall = tight cluster. Large = wide spread." },
                    new() { Title = "Probability Basics", LessonOrder = 3, Content = "Probability scales 0 to 1. Equally likely outcomes: favourable / total. Rolling a 4 = 1/6.\n\nIndependent events multiply: two heads in a row = 1/4." },
                    new() { Title = "Normal Distribution", LessonOrder = 4, Content = "Bell curve, described by mean and standard deviation.\n\n68-95-99.7 rule: about 68% within 1 SD, 95% within 2, 99.7% within 3." },
                },
                Quizzes = new List<Quiz>
                {
                    new()
                    {
                        Title = "Statistics Quiz",
                        Description = "Averages, spread and probability.",
                        Questions = new List<Question>
                        {
                            new() { QuestionText = "Least affected by outliers?", OptionA = "Mean", OptionB = "Median", OptionC = "Range", OptionD = "Sum", CorrectOption = "B" },
                            new() { QuestionText = "Standard deviation is sqrt of?", OptionA = "Mean", OptionB = "Mode", OptionC = "Variance", OptionD = "Median", CorrectOption = "C" },
                            new() { QuestionText = "P(rolling a 4)?", OptionA = "1/2", OptionB = "1/4", OptionC = "1/6", OptionD = "1/3", CorrectOption = "C" },
                            new() { QuestionText = "Within 1 SD is what %?", OptionA = "50%", OptionB = "68%", OptionC = "95%", OptionD = "99.7%", CorrectOption = "B" },
                            new() { QuestionText = "Independent events?", OptionA = "One changes other's odds", OptionB = "Always equal", OptionC = "One doesn't affect other", OptionD = "Never both", CorrectOption = "C" },
                        }
                    }
                }
            };

            var physics = new Course
            {
                Title = "Physics: Forces and Motion",
                Description = "Newton's laws with everyday examples: seatbelts, rockets, falling feathers.",
                Level = "Beginner",
                CategoryId = science.CategoryId,
                Lessons = new List<Lesson>
                {
                    new() { Title = "Newton's Three Laws", LessonOrder = 1, Content = "1: Objects stay at rest or constant velocity unless acted on by a net force.\n2: F = ma.\n3: Every force has an equal and opposite reaction." },
                    new() { Title = "Speed, Velocity, Acceleration", LessonOrder = 2, Content = "Speed = how fast. Velocity = speed + direction. Acceleration = change in velocity.\n\nTurning at constant speed is still acceleration." },
                    new() { Title = "Gravity and Free Fall", LessonOrder = 3, Content = "Gravity accelerates all objects near Earth at ~9.8 m/s\u00b2, regardless of mass.\n\nFeather falls slower in air due to air resistance, not gravity. In a vacuum they fall together." },
                    new() { Title = "Momentum and Collisions", LessonOrder = 4, Content = "Momentum = mass \u00d7 velocity. Conserved in a closed system.\n\nElastic collision: kinetic energy also conserved. Inelastic: some lost to heat/deformation." },
                },
                Quizzes = new List<Quiz>
                {
                    new()
                    {
                        Title = "Forces and Motion Quiz",
                        Description = "Newton's laws, motion, momentum.",
                        Questions = new List<Question>
                        {
                            new() { QuestionText = "F = ma is which law?", OptionA = "First", OptionB = "Second", OptionC = "Third", OptionD = "Gravitation", CorrectOption = "B" },
                            new() { QuestionText = "Velocity vs speed?", OptionA = "Velocity has direction", OptionB = "Speed larger", OptionC = "Identical", OptionD = "Only falling", CorrectOption = "A" },
                            new() { QuestionText = "Feather falls slower in air because?", OptionA = "Weaker gravity", OptionB = "Air resistance", OptionC = "Magnetism", OptionD = "More mass", CorrectOption = "B" },
                            new() { QuestionText = "Always conserved in collision?", OptionA = "Kinetic energy", OptionB = "Momentum", OptionC = "Speed", OptionD = "Temperature", CorrectOption = "B" },
                            new() { QuestionText = "Object at rest, no net force?", OptionA = "Accelerates", OptionB = "Stays at rest", OptionC = "Spins", OptionD = "Loses mass", CorrectOption = "B" },
                        }
                    }
                }
            };

            var chemistry = new Course
            {
                Title = "Chemistry Essentials",
                Description = "Atoms, bonds, balanced equations and the pH scale, from first principles.",
                Level = "Beginner",
                CategoryId = science.CategoryId,
                Lessons = new List<Lesson>
                {
                    new() { Title = "Atoms and the Periodic Table", LessonOrder = 1, Content = "Atoms: protons + neutrons in nucleus, electrons orbiting. Proton count defines element (6 = carbon).\n\nSame column in periodic table = same outer electrons = similar behaviour." },
                    new() { Title = "Chemical Bonds", LessonOrder = 2, Content = "Ionic: electron transferred. Covalent: electron shared.\n\nIonic compounds: hard, high melting. Covalent molecules: often gases or liquids at room temp." },
                    new() { Title = "Balancing Chemical Equations", LessonOrder = 3, Content = "Same atoms on both sides. 2H2 + O2 \u2192 2H2O is balanced.\n\nAdjust coefficients, never subscripts (H2O vs H2O2 = different substances)." },
                    new() { Title = "Acids and Bases", LessonOrder = 4, Content = "pH 0\u201314. <7 acidic, 7 neutral, >7 basic.\n\nAcids release H+, bases release OH-. Each pH step = 10\u00d7 change in acidity." },
                },
                Quizzes = new List<Quiz>
                {
                    new()
                    {
                        Title = "Chemistry Basics Quiz",
                        Description = "Atoms, bonds, equations, pH.",
                        Questions = new List<Question>
                        {
                            new() { QuestionText = "What defines an element?", OptionA = "Neutrons", OptionB = "Protons", OptionC = "Electrons only", OptionD = "Mass", CorrectOption = "B" },
                            new() { QuestionText = "Electrons transferred = what bond?", OptionA = "Covalent", OptionB = "Metallic", OptionC = "Ionic", OptionD = "Hydrogen", CorrectOption = "C" },
                            new() { QuestionText = "H atoms in 2H2 + O2 \u2192 2H2O?", OptionA = "2", OptionB = "4", OptionC = "6", OptionD = "8", CorrectOption = "B" },
                            new() { QuestionText = "pH 3 vs pH 4: how much more acidic?", OptionA = "2x", OptionB = "3x", OptionC = "10x", OptionD = "Same", CorrectOption = "C" },
                            new() { QuestionText = "Bases release which ion?", OptionA = "H+", OptionB = "OH-", OptionC = "O2-", OptionD = "Na+", CorrectOption = "B" },
                        }
                    }
                }
            };

            context.Courses.AddRange(python, javascript, algebra, statistics, physics, chemistry);
            await context.SaveChangesAsync();

            // 5. Enrolments, quiz attempts, discussion posts
            var members = await context.Users.Where(u => u.Role == "Member").OrderBy(u => u.UserId).ToListAsync();

            if (members.Count >= 5)
            {
                var aisha = members[0];
                var daniel = members[1];
                var priya = members[2];
                var tom = members[3];
                var maria = members[4];

                context.Enrollments.AddRange(
                    new Enrollment { UserId = aisha.UserId, CourseId = python.CourseId, IsCompleted = true, EnrolledAt = DateTime.Now.AddDays(-30) },
                    new Enrollment { UserId = aisha.UserId, CourseId = javascript.CourseId, IsCompleted = false, EnrolledAt = DateTime.Now.AddDays(-6) },
                    new Enrollment { UserId = daniel.UserId, CourseId = algebra.CourseId, IsCompleted = true, EnrolledAt = DateTime.Now.AddDays(-21) },
                    new Enrollment { UserId = daniel.UserId, CourseId = statistics.CourseId, IsCompleted = false, EnrolledAt = DateTime.Now.AddDays(-4) },
                    new Enrollment { UserId = priya.UserId, CourseId = physics.CourseId, IsCompleted = false, EnrolledAt = DateTime.Now.AddDays(-9) },
                    new Enrollment { UserId = priya.UserId, CourseId = chemistry.CourseId, IsCompleted = true, EnrolledAt = DateTime.Now.AddDays(-18) },
                    new Enrollment { UserId = tom.UserId, CourseId = javascript.CourseId, IsCompleted = false, EnrolledAt = DateTime.Now.AddDays(-2) },
                    new Enrollment { UserId = maria.UserId, CourseId = python.CourseId, IsCompleted = false, EnrolledAt = DateTime.Now.AddDays(-3) },
                    new Enrollment { UserId = maria.UserId, CourseId = statistics.CourseId, IsCompleted = true, EnrolledAt = DateTime.Now.AddDays(-25) }
                );

                var pythonQuiz = await context.Quizzes.FirstAsync(q => q.Title == "Python Basics Quiz");
                var algebraQuiz = await context.Quizzes.FirstAsync(q => q.Title == "Algebra Basics Quiz");
                var chemistryQuiz = await context.Quizzes.FirstAsync(q => q.Title == "Chemistry Basics Quiz");
                var statisticsQuiz = await context.Quizzes.FirstAsync(q => q.Title == "Statistics Quiz");

                context.QuizAttempts.AddRange(
                    new QuizAttempt { UserId = aisha.UserId, QuizId = pythonQuiz.QuizId, Score = 4, TotalQuestions = 5, AttemptedAt = DateTime.Now.AddDays(-28) },
                    new QuizAttempt { UserId = daniel.UserId, QuizId = algebraQuiz.QuizId, Score = 5, TotalQuestions = 5, AttemptedAt = DateTime.Now.AddDays(-19) },
                    new QuizAttempt { UserId = priya.UserId, QuizId = chemistryQuiz.QuizId, Score = 3, TotalQuestions = 5, AttemptedAt = DateTime.Now.AddDays(-16) },
                    new QuizAttempt { UserId = maria.UserId, QuizId = statisticsQuiz.QuizId, Score = 4, TotalQuestions = 5, AttemptedAt = DateTime.Now.AddDays(-22) }
                );

                context.DiscussionPosts.AddRange(
                    new DiscussionPost { UserId = aisha.UserId, CourseId = python.CourseId, Message = "The file-reading lesson finally made the 'with' statement click for me.", PostedAt = DateTime.Now.AddDays(-27) },
                    new DiscussionPost { UserId = daniel.UserId, CourseId = algebra.CourseId, Message = "Struggled with the inequality flip rule for years and this sorted it out in one read.", PostedAt = DateTime.Now.AddDays(-20) },
                    new DiscussionPost { UserId = priya.UserId, CourseId = physics.CourseId, Message = "Would it be possible to add a short lesson on projectile motion?", PostedAt = DateTime.Now.AddDays(-8) },
                    new DiscussionPost { UserId = tom.UserId, CourseId = javascript.CourseId, Message = "The fetch() example threw a CORS error against a different API - worth noting.", PostedAt = DateTime.Now.AddDays(-1) },
                    new DiscussionPost { UserId = maria.UserId, CourseId = statistics.CourseId, Message = "Working through the standard deviation lesson with my own data made it stick.", PostedAt = DateTime.Now.AddDays(-23) }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}