# AI Assistant Instructions for F# and React Projects

## General Guidelines

- Provide concise, practical solutions
- Follow my instructions precisely
- Prioritize compiler-friendly code over theoretical patterns
- Consider performance implications of suggestions

## F# Coding Guidelines

- Use idiomatic F# with functional programming patterns
- Prefer immutable data structures and pure functions
- Use type inference where it improves readability, explicit types where it improves clarity
- Leverage the F# type system for safety (discriminated unions, record types, etc.)
- Follow F# formatting conventions (4-space indentation, careful line breaks)
- Use pattern matching over conditional expressions when appropriate
- Prefer pipeline operators (|>, ||>) for data transformation flows
- Use higher-order functions (map, filter, fold) over explicit recursion when possible
- Leverage computation expressions where appropriate (async, task, result, etc.)

## React SPA Specific Guidelines

- Focus on modern React patterns with functional components and hooks
- Follow React best practices for state management (useState, useReducer, useContext)
- Implement proper component composition and reusability
- Use TypeScript for type safety in the React frontend
- Prefer types over interfaces for component props and state definitions
- Avoid using default exports, prefer named exports for all components and utilities
- Always use styled-components with Emotion for styling (avoid CSS modules or plain CSS)
- Use the Emotion css prop or styled API consistently throughout the project
- Follow component-centric styling patterns with the styled API
- Apply proper prop typing with explicit type declarations
- Implement effective CSS strategies (CSS modules, styled-components, etc.)
- Structure components with clear separation of concerns
- Consider performance optimization techniques (React.memo, useMemo, useCallback)
- Follow recommended patterns for handling side effects (useEffect)
- Implement proper error boundaries and error handling
- Use modern React routing solutions (React Router)
- Implement effective form management strategies
- Accessibility is not a priority for this project at this time

## Code Organization

- Respect the existing project structure
- Separate concerns appropriately (domain logic, UI logic, effects)
- Consider domain-driven design principles for business logic

## Testing Suggestions

- Include testing suggestions using Expecto, FsCheck, or other F# testing libraries
- For React components, suggest Jest, React Testing Library, or Cypress approaches
- Focus on component unit tests, integration tests, and E2E testing where appropriate

## Performance Considerations

- Be aware of boxing/unboxing and unnecessary allocations
- Consider tail-call optimization for recursive functions
- Be mindful of lazy evaluation where appropriate
- Suggest appropriate memoization techniques when beneficial

## Documentation

- Include XML documentation for public functions/types
- Explain complex algorithms or patterns in comments
- Document assumptions and edge cases

## Output Format

- Structure responses with relevant sections (Problem, Solution, Explanation)
- Use syntax-highlighted code blocks with appropriate language tags
- Comment code thoroughly, especially for complex logic
- Avoid repeating existing code, use `// ...existing code...` notation
- Present multiple approaches when appropriate, with pros and cons
